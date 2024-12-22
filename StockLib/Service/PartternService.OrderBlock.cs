using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task OrderBlock(string code)
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
                OrderBlock(lData.TakeLast(300).ToList());
                //for (int i = 300; i < count; i++)
                //{
                //    var skip = i - 300;
                //    if (skip < 0)
                //        skip = 0;
                //    OrderBlock(lData.Skip(skip).Take(i).ToList());
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.OrderBlock|EXCEPTION| {ex.Message}");
            }
        }

        public void OrderBlock(List<Quote> lData)
        {
            try
            {
                var lTopDown = lData.GetTopBottom_HL_TopClean(0, false);
                var lTop = lTopDown.Where(x => x.IsTop);
                var lBot = lTopDown.Where(x => x.IsBot);
                foreach (var top in lTop) 
                {
                    var item = lData.First(x => x.Date == top.Date);
                    var uplen = item.High - Math.Max(item.Open, item.Close);
                    var len = item.High - item.Low;

                    if (uplen / len >= (decimal)0.4)
                    {
                        var entry = item.High - uplen / 2;
                        var sl = entry + uplen;
                        Console.WriteLine($"TOP(pinbar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                    }
                    else
                    {
                        var index = lData.IndexOf(item);
                        var next = lData.ElementAt(index + 1);
                        if (next.Open > next.Close && next.Close <= Math.Min(item.Open, item.Close) && next.Open >= Math.Max(item.Open, item.Close))
                        {
                            var entry = Math.Min(item.Open, item.Close);
                            var sl = Math.Max(item.High, next.High);
                            Console.WriteLine($"TOP(outsidebar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                        }
                    }
                }
                foreach (var top in lBot)
                {
                    var item = lData.First(x => x.Date == top.Date);
                    var downlen = Math.Min(item.Open, item.Close) - item.Low;
                    var len = item.High - item.Low;

                    if (downlen / len >= (decimal)0.4)
                    {
                        var entry = downlen / 2 + item.Low;
                        var sl = entry - downlen;
                        Console.WriteLine($"BOT(pinbar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                    }
                    else
                    {
                        var index = lData.IndexOf(item);
                        var next = lData.ElementAt(index + 1);
                        if (next.Open < next.Close && next.Close >= Math.Max(item.Open, item.Close) && next.Open <= Math.Min(item.Open, item.Close))
                        {
                            var entry = Math.Max(item.Open, item.Close);
                            var sl = Math.Min(item.Low, next.Low);
                            Console.WriteLine($"BOT(outsidebar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                        }
                    }
                }
                var tmp = 1;


                //var BatDay = lastItem.CheckBatDay(lastVol, lastBB);
                //if (BatDay.Item1)
                //{
                //    Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {BatDay.Item2}");
                //}

                //lastItem = lData.SkipLast(1).Last();
                //lastVol = lVolMa.SkipLast(1).Last();
                //lastBB = lBB.SkipLast(1).Last();
                //BatDay = lastItem.CheckBatDay(lastVol, lastBB);
                //if (BatDay.Item1)
                //{
                //    Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {BatDay.Item2}");
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.BatDay|EXCEPTION| {ex.Message}");
            }
        }
    }

    public static class clsOrderBlock
    {
        public static (bool, decimal) CheckOrderBlock(this Quote item, SmaResult vol, BollingerBandsResult bb)
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

    }
}
