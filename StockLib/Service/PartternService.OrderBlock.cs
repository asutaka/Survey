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

        public List<QuoteEx> OrderBlock(List<Quote> lData)
        {
            try
            {
                var lTopDown = lData.GetTopBottom_HL_TopClean(0, false);
                var lTop = lTopDown.Where(x => x.IsTop);
                var lBot = lTopDown.Where(x => x.IsBot);
                var lOrderBlockTop = new List<QuoteEx>();
                var lOrderBlockBot = new List<QuoteEx>();
                var maxTop = lTop.MaxBy(x => x.Value);
                var minBot = lBot.MinBy(x => x.Value);
                lTop = lTop.Where(x => x.Date >= maxTop.Date).ToList();
                lBot = lBot.Where(x => x.Date >= minBot.Date).ToList();
                foreach (var top in lTop) 
                {
                    var item = lData.First(x => x.Date == top.Date);
                    var uplen = item.High - Math.Max(item.Open, item.Close);
                    var len = item.High - item.Low;
                    var avg = lData.Where(x => x.Date <= top.Date).TakeLast(5).Average(x => x.High - x.Low);
                    var itemData = lData.First(x => x.Date == top.Date);

                    if (uplen / len >= (decimal)0.6 
                        && (itemData.High - itemData.Low) >= (decimal)1.3 * avg)
                    {
                        var entry = item.High - uplen / 4;
                        var sl = entry + uplen;
                        lOrderBlockTop.Add(new QuoteEx
                        {
                            Date = item.Date,
                            Open = item.Open,
                            Close = item.Close,
                            High = item.High,
                            Low = item.Low,
                            Mode = (int)EOrderBlockMode.TopPinbar,
                            Entry = entry,
                            SL = sl,
                            Focus = Math.Max(item.Open, item.Close),
                        });
                        //Console.WriteLine($"TOP(pinbar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                    }
                    else
                    {
                        var index = lData.IndexOf(item);
                        var next = lData.ElementAt(index + 1);
                        if (next.Open > next.Close 
                            && next.Close <= Math.Min(item.Open, item.Close) 
                            && next.Open >= Math.Max(item.Open, item.Close)
                            && (next.High - next.Low) >= (decimal)1.3 * avg)
                        {
                            var entry = Math.Min(item.Open, item.Close) + 3 * Math.Abs(item.Open - item.Close) / 4;
                            var sl = Math.Max(item.High, next.High) + Math.Abs(item.Open - item.Close) / 4; 
                            lOrderBlockTop.Add(new QuoteEx
                            {
                                Date = item.Date,
                                Open = item.Open,
                                Close = item.Close,
                                High = item.High,
                                Low = item.Low,
                                Mode = (int)EOrderBlockMode.TopInsideBar,
                                Entry = entry,
                                SL = sl,
                                Focus = Math.Min(item.Open, item.Close)
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
                    var avg = lData.Where(x => x.Date <= top.Date).TakeLast(5).Average(x => x.High - x.Low);
                    var itemData = lData.First(x => x.Date == top.Date);

                    if (downlen / len >= (decimal)0.6
                        && (itemData.High - itemData.Low) >= (decimal)1.3 * avg)
                    {
                        var entry = downlen / 4 + item.Low;
                        var sl = entry - downlen;
                        //Console.WriteLine($"BOT(pinbar): {item.Date.ToString("dd/MM/yyyy HH:mm")}|ENTRY: {entry}|SL: {sl}");
                        lOrderBlockBot.Add(new QuoteEx
                        {
                            Date = item.Date,
                            Open = item.Open,
                            Close = item.Close,
                            High = item.High,
                            Low = item.Low,
                            Mode = (int)EOrderBlockMode.BotPinbar,
                            Entry = entry,
                            SL = sl,
                            Focus = Math.Max(item.Open, item.Close)
                        });
                    }
                    else
                    {
                        var index = lData.IndexOf(item);
                        var next = lData.ElementAt(index + 1);
                        if (next.Open < next.Close 
                            && next.Close >= Math.Max(item.Open, item.Close) 
                            && next.Open <= Math.Min(item.Open, item.Close)
                            && (next.High - next.Low) >= (decimal)1.3 * avg)
                        {
                            var entry = Math.Min(item.Open, item.Close) + Math.Abs(item.Open - item.Close) / 4;
                            var sl = Math.Min(item.Low, next.Low) + Math.Abs(item.Open - item.Close) / 4; ;
                            lOrderBlockBot.Add(new QuoteEx
                            {
                                Date = item.Date,
                                Open = item.Open,
                                Close = item.Close,
                                High = item.High,
                                Low = item.Low,
                                Mode = (int)EOrderBlockMode.BotInsideBar,
                                Entry = entry,
                                SL = sl,
                                Focus = Math.Max(item.Open, item.Close)
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
                return lTotal;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.OrderBlock|EXCEPTION| {ex.Message}");
            }
            return null;
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
