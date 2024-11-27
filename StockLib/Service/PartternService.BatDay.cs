using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task BatDay(string code)
        {
            try
            {
                _code = code;
                var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Linear, code, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 1000);
                var lData = lByBit.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lData.Reverse();

                var count = lData.Count();
                BatDay(lData);
                //for (int i = 300; i < count; i++)
                //{
                //    BatDay(lData.Take(i).ToList());
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.BatDay|EXCEPTION| {ex.Message}");
            }
        }

        public void BatDay(List<Quote> lData)
        {
            try
            {
                var lVol = new List<Quote>();
                foreach (var item in lData) 
                {
                    lVol.Add(new Quote { Date = item.Date, Close = item.Volume, Open = item.Open, High = item.High, Low = item.Low, Volume = item.Volume });
                }
                var lVolMa = lVol.GetSma(20);
                var lBB = lData.GetBollingerBands();

                var lastItem = lData.Last();
                var lastVol = lVolMa.Last();
                var lastBB = lBB.Last();

                var Trendline = lData.CheckTrendline(lVolMa, lBB);
                if (Trendline.Item1)
                {
                    Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {Trendline.Item2}");
                }

                var BatDay = lastItem.CheckBatDay(lastVol, lastBB);
                if (BatDay.Item1)
                {
                    Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {BatDay.Item2}");
                }

                lastItem = lData.SkipLast(1).Last();
                lastVol = lVolMa.SkipLast(1).Last();
                lastBB = lBB.SkipLast(1).Last();
                BatDay = lastItem.CheckBatDay(lastVol, lastBB);
                if (BatDay.Item1)
                {
                    Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {BatDay.Item2}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.BatDay|EXCEPTION| {ex.Message}");
            }
        }
    }

    public static class clsBatDay
    {
        public static (bool, decimal) CheckBatDay(this Quote item, SmaResult vol, BollingerBandsResult bb)
        {
            try
            {
                if (item.Close < item.Open * (decimal)1.01)
                    return (false, 0);

                if(item.Low >= (decimal)bb.LowerBand)
                    return (false, 0);

                if (item.Close > (decimal)bb.Sma)
                    return (false, 0);

                if (item.Volume <= (decimal)(vol.Sma * 1.05)) 
                    return (false, 0);

                if ((item.High - item.Low) * 2 >= (decimal)(bb.UpperBand - bb.LowerBand))
                    return (false, 0);

                return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PartternService.CheckBatDay|EXCEPTION| {ex.Message}");
            }
            return (false, 0);
        }

        public static (bool, decimal) CheckTrendline(this List<Quote> lData, IEnumerable<SmaResult> lVol, IEnumerable<BollingerBandsResult> lBB)
        {
            try
            {
                var lTop = lData.GetTopBottom_HL_TopClean(0, false).Where(x => x.IsTop);
                var item = lData.Last();
                var near = lData.Skip(1).Last();
                var near2 = lData.Skip(2).Last();
                if (!((item.Close > item.Open * (decimal)1.01 || near.Close > near.Open * (decimal)1.01)
                    || (near2.Close > near2.Open * (decimal)1.01 || near.Close > near.Open * (decimal)1.01)))
                {
                    return (false, 0);
                }

                var curTop = lTop.Last();
                var lTake = lTop.Where(x => x.Value > curTop.Value).TakeLast(5).ToList();
                lTake.Reverse();
                foreach (var take in lTake)
                {
                    var index = lData.IndexOf(item);
                    var indexTake = lData.IndexOf(lData.First(x => x.Date == take.Date));
                    var indexCur = lData.IndexOf(lData.First(x => x.Date == curTop.Date));
                    var check = isTrendline(item, near, take.Value, curTop.Value, index, indexTake, indexCur);
                    if (check)
                    {
                        Console.WriteLine($"Top: {take.Date.ToString("dd/MM/yyyy HH")}|CurTop: {curTop.Date.ToString("dd/MM/yyyy HH")}");
                        return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                    }
                    check = isTrendline(near, near2, take.Value, curTop.Value, index - 1, indexTake, indexCur);
                    if (check)
                    {
                        Console.WriteLine($"(2)Top: {take.Date.ToString("dd/MM/yyyy HH")}|CurTop: {curTop.Date.ToString("dd/MM/yyyy HH")}");
                        return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PartternService.CheckTrendline|EXCEPTION| {ex.Message}");
            }
            return (false, 0);
        }

        private static bool isTrendline(Quote cur, Quote near, decimal max1, decimal max2, int indexCur, int indexMax1, int indexMax2)
        {
            var curCheck = (max1 - max2) * (indexCur - indexMax1) + (indexMax2 - indexMax1) * (cur.Close - max1) > 0;
            var nearCheck = (max1 - max2) * ((indexCur - 1) - indexMax1) + (indexMax2 - indexMax1) * (near.Close - max1) > 0;
            var res = curCheck && !nearCheck;
            if (res)
            {
                //var entry = (decimal)1.005 * (max1 + ((max1 - max2) * (indexCur - indexMax1) / (indexMax1 - indexMax2)));
                //Console.WriteLine($"{cur.Close}|{entry}");
            }
            return res;
        }

    }
}
