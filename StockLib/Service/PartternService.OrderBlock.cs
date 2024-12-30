using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Model;
using StockLib.Utils;
using static iTextSharp.text.pdf.AcroFields;

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
                var lOrderBlockTop = new List<QuoteEx>();
                var lOrderBlockBot = new List<QuoteEx>();
                foreach (var top in lTop) 
                {
                    var item = lData.First(x => x.Date == top.Date);
                    var uplen = item.High - Math.Max(item.Open, item.Close);
                    var len = item.High - item.Low;

                    if (uplen / len >= (decimal)0.4)
                    {
                        var entry = item.High - uplen / 2;
                        var sl = entry + uplen;
                        lOrderBlockTop.Add(new QuoteEx
                        {
                            Date = item.Date,
                            Open = item.Open,
                            Close = item.Close,
                            High = item.High,
                            Low = item.Low,
                            Mode = 1,
                            Entry = entry,
                            SL = sl,
                        });
                        //Console.WriteLine($"TOP(pinbar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                    }
                    else
                    {
                        var index = lData.IndexOf(item);
                        var next = lData.ElementAt(index + 1);
                        if (next.Open > next.Close && next.Close <= Math.Min(item.Open, item.Close) && next.Open >= Math.Max(item.Open, item.Close))
                        {
                            var entry = Math.Min(item.Open, item.Close);
                            var sl = Math.Max(item.High, next.High);
                            lOrderBlockTop.Add(new QuoteEx
                            {
                                Date = item.Date,
                                Open = item.Open,
                                Close = item.Close,
                                High = item.High,
                                Low = item.Low,
                                Mode = 2,
                                Entry = entry,
                                SL = sl,
                            });
                            //Console.WriteLine($"TOP(outsidebar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
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
                        //Console.WriteLine($"BOT(pinbar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                        lOrderBlockBot.Add(new QuoteEx
                        {
                            Date = item.Date,
                            Open = item.Open,
                            Close = item.Close,
                            High = item.High,
                            Low = item.Low,
                            Mode = 3,
                            Entry = entry,
                            SL = sl,
                        });
                    }
                    else
                    {
                        var index = lData.IndexOf(item);
                        var next = lData.ElementAt(index + 1);
                        if (next.Open < next.Close && next.Close >= Math.Max(item.Open, item.Close) && next.Open <= Math.Min(item.Open, item.Close))
                        {
                            var entry = Math.Max(item.Open, item.Close);
                            var sl = Math.Min(item.Low, next.Low);
                            lOrderBlockBot.Add(new QuoteEx
                            {
                                Date = item.Date,
                                Open = item.Open,
                                Close = item.Close,
                                High = item.High,
                                Low = item.Low,
                                Mode = 4,
                                Entry = entry,
                                SL = sl,
                            });
                            //Console.WriteLine($"BOT(outsidebar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                        }
                    }
                }

                var lRemove = new List<QuoteEx>();
                var countTop = lOrderBlockTop.Count;
                if (countTop > 0) 
                {
                    for (var i = 0; i < countTop - 1; i++)
                    {
                        var cur = lOrderBlockTop[i];
                        var next = lOrderBlockTop[i + 1];
                        if (next.High >= cur.High)
                        {
                            lRemove.Add(cur);
                        }
                    }
                    if(lRemove.Any())
                    {
                        lOrderBlockTop = lOrderBlockTop.Where(x => !lRemove.Any(y => y.Date == x.Date)).ToList();
                        lRemove.Clear();
                    }
                }

                var countBot = lOrderBlockBot.Count;
                if (countBot > 0)
                {
                    for (var i = 0; i < countBot - 1; i++)
                    {
                        var cur = lOrderBlockBot[i];
                        var next = lOrderBlockBot[i + 1];
                        if (next.Low <= cur.Low)
                        {
                            lRemove.Add(cur);
                        }
                    }
                    if (lRemove.Any())
                    {
                        lOrderBlockBot = lOrderBlockBot.Where(x => !lRemove.Any(y => y.Date == x.Date)).ToList();
                    }
                }

                var lTotal = new List<QuoteEx>();
                lTotal.AddRange(lOrderBlockTop);
                lTotal.AddRange(lOrderBlockBot);
                lTotal = lTotal.OrderBy(x => x.Date).ToList();
                foreach (var item in lTotal) 
                {
                    var title = string.Empty;
                    if (item.Mode == 1)
                    {
                        title = "TOP(pinbar)";
                    }
                    else if (item.Mode == 2)
                    {
                        title = "TOP(outsidebar)";
                    }
                    else if (item.Mode == 3)
                    {
                        title = "BOT(pinbar)";
                    }
                    else title = "BOT(outsidebar)";
                    Console.WriteLine($"{title}: {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {item.Entry}|SL: {item.SL}");
                }
                //Console.WriteLine($"TOP(pinbar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");

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
