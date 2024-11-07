using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyPriceAction(string code)
        {
            try
            {
                var lAll = await _apiService.GetCoinData_Binance(code, 200, $"4h");
                var count = lAll.Count;
                for (int j = 6; j < count - 1; j++)
                {
                    var lData = lAll;
                    //var lData = lAll.Take(j).ToList();
                    var res = lData.CheckPriceAction();
                    if(res.Item1)
                    {
                        var mode = res.Item2 == 1 ? "BUY" : "SELL";
                        Console.WriteLine($"{mode} {code}: {lData.Last().Date.ToString("dd/MM/yyyy HH")}");
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyEliot|EXCEPTION| {ex.Message}");
            }
        }
    }

    public static class clsPriceAction
    {
        public static (bool, int) CheckPriceAction(this List<Quote> lVal)
        {
            try
            {
                if (lVal.Count() < 100)
                    return (false, 0);

                var cur = lVal.Last();
                var count = lVal.Count();
                var near1 = lVal.SkipLast(1).Last();
                var near2 = lVal.SkipLast(2).Last();
                var lTopBottom = lVal.GetTopBottomClean_HL(4, true);
                var lTop = lTopBottom.Where(x => x.IsTop);
                var lBot = lTopBottom.Where(x => x.IsBot);
                var TopBottom = lTopBottom.Last();
                if (TopBottom.IsBot)
                {
                    var Top = lTop.Last();
                    var eMax = lVal.TakeLast(50).MaxBy(x => x.High);
                    var div = (Top.Date - eMax.Date).TotalMinutes;
                    if (div < 0)
                        return (false, 0);
                    var itemTop = lVal.First(x => x.Date == Top.Date);
                    var itemMax = lVal.First(x => x.Date == eMax.Date);
                    var indexTop = lVal.IndexOf(itemTop);
                    var indexMax = lVal.IndexOf(itemMax);
                    var c = indexTop - indexMax;
                    if(c < 10)
                        return (false, 0);
                    var x0 = (count - 1) - indexMax;
                    if((x0 - c) <= 5) return (false, 0);

                    var val = (eMax.High - Top.Value) * x0 + c * (cur.Close - eMax.High);
                    if (val <= 0)
                        return (false, 0);

                    var x1 = (count - 2) - indexMax;
                    var x2 = (count - 3) - indexMax;
                    var val1 = (eMax.High - Top.Value) * x1 + c * (near1.Close - eMax.High);
                    var val2 = (eMax.High - Top.Value) * x2 + c * (near2.Close - eMax.High);

                    return ((val1 < 0 || val2 < 0), 1);
                }
                else
                {
                    var Bot = lBot.Last();
                    var eMin = lVal.TakeLast(50).MinBy(x => x.Low);
                    var div = (Bot.Date - eMin.Date).TotalMinutes;
                    if (div < 0)
                        return (false, 0);
                    var itemBot = lVal.First(x => x.Date == Bot.Date);
                    var itemMin = lVal.First(x => x.Date == eMin.Date);
                    var indexBot = lVal.IndexOf(itemBot);
                    var indexMin = lVal.IndexOf(itemMin);
                    var c = indexBot - indexMin;
                    if (c < 10)
                        return (false, 0);
                    var x0 = (count - 1) - indexMin;
                    if ((x0 - c) <= 5) return (false, 0);

                    var val = (eMin.Low - Bot.Value) * x0 + c * (cur.Close - eMin.Low);
                    if (val >= 0)
                        return (false, 0);

                    var x1 = (count - 2) - indexMin;
                    var x2 = (count - 3) - indexMin;
                    var val1 = (eMin.Low - Bot.Value) * x1 + c * (near1.Close - eMin.Low);
                    var val2 = (eMin.Low - Bot.Value) * x2 + c * (near2.Close - eMin.Low);

                    return ((val1 > 0 || val2 > 0), 2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PartternService.CheckPriceAction|EXCEPTION| {ex.Message}");
            }
            return (false, 0);
        }
    }
}
