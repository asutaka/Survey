using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;
using static iTextSharp.text.pdf.AcroFields;
using System;

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
                for (int i = 50; i < count; i++)
                {
                    BatDay(lData.Take(i).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.BatDay|EXCEPTION| {ex.Message}");
            }
        }

        private void BatDay(List<Quote> lData)
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

                var Trendline = lData.CheckTrendline(lVolMa, lBB);
                if (Trendline.Item1)
                {
                    Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {Trendline.Item2}");
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
                var item = lData.Last();
                var near = lData.Skip(1).Last();
                var near2 = lData.Skip(2).Last();
                var max150 = lData.TakeLast(150).MaxBy(x => x.High);
                var max100 = lData.TakeLast(100).MaxBy(x => x.High);
                var max80 = lData.TakeLast(80).MaxBy(x => x.High);
                var max50 = lData.TakeLast(50).MaxBy(x => x.High);
                var max30 = lData.TakeLast(30).MaxBy(x => x.High);

                var index = lData.IndexOf(item);
                var index150 = lData.IndexOf(max150);
                var index100 = lData.IndexOf(max100);
                var index80 = lData.IndexOf(max80);
                var index50 = lData.IndexOf(max50);
                var index30 = lData.IndexOf(max30);

                
                //150
                if(index150 < index80 && max150.High > max80.High)
                {
                    var check = isTrendline(item, near, max150, max80, index, index150, index80);
                    if (check)
                    {
                        Console.WriteLine("150 -> 80");
                        return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                    }
                    check = isTrendline(near, near2, max150, max80, index - 1, index150, index80);
                    if (check)
                    {
                        Console.WriteLine("150 -> 80: 2");
                        return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                    }
                }

                if (index150 < index50 && max150.High > max50.High)
                {
                    var check = isTrendline(item, near, max150, max50, index, index150, index50);
                    if (check)
                    {
                        Console.WriteLine("150 -> 50");
                        return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                    }
                    check = isTrendline(near, near2, max150, max50, index - 1, index150, index50);
                    if (check)
                    {
                        Console.WriteLine("150 -> 50: 2");
                        return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                    }
                }

                if (index150 < index30 && max150.High > max30.High)
                {
                    var check = isTrendline(item, near, max150, max30, index, index150, index30);
                    if (check)
                    {
                        Console.WriteLine("150 -> 80");
                        return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                    }
                    check = isTrendline(near, near2, max150, max30, index - 1, index150, index30);
                    if (check)
                    {
                        Console.WriteLine("150 -> 80: 2");
                        return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                    }
                }
                //100
                if (index100 < index50 && max100.High > max50.High)
                {
                    var check = isTrendline(item, near, max100, max50, index, index100, index50);
                    if (check)
                    {
                        Console.WriteLine("100 -> 50");
                        return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                    }
                    check = isTrendline(near, near2, max100, max50, index - 1, index100, index50);
                    if (check)
                    {
                        Console.WriteLine("100 -> 50: 2");
                        return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                    }
                }

                if (index100 < index30 && max100.High > max30.High)
                {
                    var check = isTrendline(item, near, max100, max30, index, index100, index30);
                    if (check)
                    {
                        Console.WriteLine("100 -> 30");
                        return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                    }
                    check = isTrendline(near, near2, max100, max30, index - 1, index100, index30);
                    if (check)
                    {
                        Console.WriteLine("100 -> 30: 2");
                        return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                    }
                }
                //80
                if (index80 < index30 && max80.High > max30.High)
                {
                    var check = isTrendline(item, near, max80, max30, index, index80, index30);
                    if (check)
                    {
                        Console.WriteLine("80 -> 30");
                        return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                    }
                    check = isTrendline(near, near2, max80, max30, index - 1, index80, index30);
                    if (check)
                    {
                        Console.WriteLine("80 -> 30: 2");
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

        private static bool isTrendline(Quote cur, Quote near, Quote max1, Quote max2, int indexCur, int indexMax1, int indexMax2)
        {
            var curCheck = (max1.High - max2.High) * (indexCur - indexMax1) + (indexMax2 - indexMax1) * (cur.Close - max1.High) > 0;
            var nearCheck = (max1.High - max2.High) * ((indexCur - 1) - indexMax1) + (indexMax2 - indexMax1) * (near.Close - max1.High) > 0;
            return curCheck && !nearCheck;
        }

    }
}
