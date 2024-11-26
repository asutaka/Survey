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
                for (int i = 300; i < count; i++)
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
                var lTop = lData.GetTopBottom_HL_TopClean(0, false).Where(x => x.IsTop);
                var item = lData.Last();
                var near = lData.Skip(1).Last();
                var near2 = lData.Skip(2).Last();
                if (!((item.Close > item.Open * (decimal)1.01 || near.Close > near.Open * (decimal)1.01)
                    || (near2.Close > near2.Open * (decimal)1.01 || near.Close > near.Open * (decimal)1.01)))
                {
                    return (false, 0);
                }

                var max = lTop.MaxBy(x => x.Value);
                var max250 = lTop.Where(x => x.Date > lData.SkipLast(250).Last().Date).MaxBy(x => x.Value);
                var max200 = lTop.Where(x => x.Date > lData.SkipLast(200).Last().Date).MaxBy(x => x.Value);
                var max150 = lTop.Where(x => x.Date > lData.SkipLast(150).Last().Date).MaxBy(x => x.Value);
                var max100 = lTop.Where(x => x.Date > lData.SkipLast(100).Last().Date).MaxBy(x => x.Value);
                var max80 = lTop.Where(x => x.Date > lData.SkipLast(80).Last().Date).MaxBy(x => x.Value);
                var max50 = lTop.Where(x => x.Date > lData.SkipLast(50).Last().Date).MaxBy(x => x.Value);
                var max30 = lTop.Where(x => x.Date > lData.SkipLast(30).Last().Date).MaxBy(x => x.Value);
                if(max50 is null)
                {
                    max50 = new TopBotModel();
                }
                if (max30 is null)
                {
                    max30 = new TopBotModel();
                }

                var index = lData.IndexOf(item);
                var indexMax = lData.IndexOf(lData.First(x => x.Date == max.Date));
                var index250 = lData.IndexOf(lData.First(x => x.Date == max250.Date));
                var index200 = lData.IndexOf(lData.First(x => x.Date == max200.Date));
                var index150 = lData.IndexOf(lData.First(x => x.Date == max150.Date));
                var index100 = lData.IndexOf(lData.First(x => x.Date == max100.Date));
                var index80 = lData.IndexOf(lData.First(x => x.Date == max80.Date));
                var index50 = lData.IndexOf(lData.FirstOrDefault(x => x.Date == max50.Date));
                var index30 = lData.IndexOf(lData.FirstOrDefault(x => x.Date == max30.Date));

                //50
                if(index - index50 >= 10)
                {
                    if (indexMax < index50 && max.Value > max50.Value)
                    {
                        var check = isTrendline(item, near, max.Value, max50.Value, index, indexMax, index50);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 50|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max.Value, max50.Value, index - 1, indexMax, index50);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 50(2)|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index250 < index50 && max250.Value > max50.Value)
                    {
                        var check = isTrendline(item, near, max250.Value, max50.Value, index, index250, index50);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 50|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max250.Value, max50.Value, index - 1, index250, index50);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 50(2)|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index200 < index50 && max200.Value > max50.Value)
                    {
                        var check = isTrendline(item, near, max200.Value, max50.Value, index, index200, index50);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 50|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max200.Value, max50.Value, index - 1, index200, index50);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 50(2)|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index150 < index50 && max150.Value > max50.Value)
                    {
                        var check = isTrendline(item, near, max150.Value, max50.Value, index, index150, index50);
                        if (check)
                        {
                            Console.WriteLine($"150 -> 50|Max1: {max150.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max150.Value, max50.Value, index - 1, index150, index50);
                        if (check)
                        {
                            Console.WriteLine($"150 -> 50(2)|Max1: {max150.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index100 < index50 && max100.Value > max50.Value)
                    {
                        var check = isTrendline(item, near, max100.Value, max50.Value, index, index100, index50);
                        if (check)
                        {
                            Console.WriteLine($"100 -> 50|Max1: {max100.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max100.Value, max50.Value, index - 1, index100, index50);
                        if (check)
                        {
                            Console.WriteLine($"100 -> 50(2)|Max1: {max100.Date.ToString("dd/MM/yyyy HH")}|Max2: {max50.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }
                }

                //30
                if(index - index30 >= 10)
                {
                    if (indexMax < index30 && max.Value > max30.Value)
                    {
                        var check = isTrendline(item, near, max.Value, max30.Value, index, indexMax, index30);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 30|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max.Value, max30.Value, index - 1, indexMax, index30);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 30(2)|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index250 < index30 && max250.Value > max30.Value)
                    {
                        var check = isTrendline(item, near, max250.Value, max30.Value, index, index250, index30);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 30|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max250.Value, max30.Value, index - 1, index250, index30);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 30(2)|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index200 < index30 && max200.Value > max30.Value)
                    {
                        var check = isTrendline(item, near, max200.Value, max30.Value, index, index200, index30);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 30|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max200.Value, max30.Value, index - 1, index200, index30);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 30(2)|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index150 < index30 && max150.Value > max30.Value)
                    {
                        var check = isTrendline(item, near, max150.Value, max30.Value, index, index150, index30);
                        if (check)
                        {
                            Console.WriteLine($"150 -> 30|Max1: {max150.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max150.Value, max30.Value, index - 1, index150, index30);
                        if (check)
                        {
                            Console.WriteLine($"150 -> 30(2)|Max1: {max150.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index100 < index30 && max100.Value > max30.Value)
                    {
                        var check = isTrendline(item, near, max100.Value, max30.Value, index, index100, index30);
                        if (check)
                        {
                            Console.WriteLine($"100 -> 30|Max1: {max100.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max100.Value, max30.Value, index - 1, index100, index30);
                        if (check)
                        {
                            Console.WriteLine($"100 -> 30(2)|Max1: {max100.Date.ToString("dd/MM/yyyy HH")}|Max2: {max30.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }
                }

                //80
                if(index - index80 >= 10)
                {
                    if (indexMax < index80 && max.Value > max80.Value)
                    {
                        var check = isTrendline(item, near, max.Value, max80.Value, index, indexMax, index80);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 80|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max.Value, max80.Value, index - 1, indexMax, index80);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 80(2)|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index250 < index80 && max250.Value > max80.Value)
                    {
                        var check = isTrendline(item, near, max250.Value, max80.Value, index, index250, index80);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 80|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max250.Value, max80.Value, index - 1, index250, index80);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 80(2)|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index200 < index80 && max200.Value > max80.Value)
                    {
                        var check = isTrendline(item, near, max200.Value, max80.Value, index, index200, index80);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 80|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max200.Value, max80.Value, index - 1, index200, index80);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 80(2)|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index150 < index80 && max150.Value > max80.Value)
                    {
                        var check = isTrendline(item, near, max150.Value, max80.Value, index, index150, index80);
                        if (check)
                        {
                            Console.WriteLine($"150 -> 80|Max1: {max150.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max150.Value, max80.Value, index - 1, index150, index80);
                        if (check)
                        {
                            Console.WriteLine($"150 -> 80(2)|Max1: {max150.Date.ToString("dd/MM/yyyy HH")}|Max2: {max80.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }
                }

                //100
                if(index - index100 >= 10)
                {
                    if (indexMax < index100 && max.Value > max100.Value)
                    {
                        var check = isTrendline(item, near, max.Value, max100.Value, index, indexMax, index100);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 100|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max100.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max.Value, max100.Value, index - 1, indexMax, index100);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 100(2)|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max100.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index250 < index100 && max250.Value > max100.Value)
                    {
                        var check = isTrendline(item, near, max250.Value, max100.Value, index, index250, index100);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 100|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max100.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max250.Value, max100.Value, index - 1, index250, index100);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 100(2)|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max100.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index200 < index100 && max200.Value > max100.Value)
                    {
                        var check = isTrendline(item, near, max200.Value, max100.Value, index, index200, index100);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 100|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max100.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max200.Value, max100.Value, index - 1, index200, index100);
                        if (check)
                        {
                            Console.WriteLine($"200 -> 100(2)|Max1: {max200.Date.ToString("dd/MM/yyyy HH")}|Max2: {max100.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }
                }

                //150
                if(index - index150 >= 10)
                {
                    if (indexMax < index150 && max.Value > max150.Value)
                    {
                        var check = isTrendline(item, near, max.Value, max150.Value, index, indexMax, index150);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 150|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max150.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max.Value, max150.Value, index - 1, indexMax, index150);
                        if (check)
                        {
                            Console.WriteLine($"Max -> 150(2)|Max1: {max.Date.ToString("dd/MM/yyyy HH")}|Max2: {max150.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
                    }

                    if (index250 < index150 && max250.Value > max150.Value)
                    {
                        var check = isTrendline(item, near, max250.Value, max150.Value, index, index250, index150);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 150|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max150.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, item.Close * (decimal)0.6 + item.Open * (decimal)0.4);
                        }
                        check = isTrendline(near, near2, max250.Value, max150.Value, index - 1, index250, index150);
                        if (check)
                        {
                            Console.WriteLine($"250 -> 150(2)|Max1: {max250.Date.ToString("dd/MM/yyyy HH")}|Max2: {max150.Date.ToString("dd/MM/yyyy HH")}");
                            return (true, near.Close * (decimal)0.6 + near.Open * (decimal)0.4);
                        }
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
            //if (res)
            //{
            //    var val = max1 + (max1 - max2) * (indexCur - indexMax1)/(indexMax1 - indexMax2);
            //    var rate = Math.Round(100 * (-1 + cur.Close / val), 3);
            //    //Console.WriteLine(rate);
            //    if (rate > (decimal)0.1)
            //        return true;
            //}
            return res;
        }

    }
}
