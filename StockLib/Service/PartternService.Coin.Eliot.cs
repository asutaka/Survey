using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        private const int _interval = 5;
        public async Task SurveyCoinEliot(string code)
        {
            try
            {
                var lAll = await _apiService.GetCoinData_Binance(code, 2000, $"4h");
                var count = lAll.Count;
                for ( int j = 6; j < count - 1; j++)
                {
                    //var lData = lAll;
                    var lData = lAll.Take(j).ToList();
                    await CheckPhanKy(lData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyEliot|EXCEPTION| {ex.Message}");
            }
        }

        private async Task<int> CheckPhanKy(List<Quote> lData)
        {
            var lRsi = lData.GetRsi(6);
            var lTopBottom = lData.GetTopBottomClean();
            //var last = lTopBottom.SkipLast(1).Last();
            //if(last.IsTop)
            //{
            //    Console.WriteLine($"TOP: {last.Date.ToString("dd/MM/yyyy HH")}");
            //}
            //if (last.IsBot)
            //{
            //    Console.WriteLine($"Bot: {last.Date.ToString("dd/MM/yyyy HH")}");
            //}

            var lTop = lTopBottom.Where(x => x.IsTop);
            if (lTop.Count() >= 2)
            {
                if (lTopBottom.SkipLast(2).Last().IsTop)
                {
                    var item = lTop.Last();
                    var prev = lTop.SkipLast(1).Last();
                    var itemRSI = lRsi.First(x => x.Date == item.Date);
                    var prevRSI = lRsi.First(x => x.Date == prev.Date);
                    if ((item.Value >= prev.Value && itemRSI.Rsi < prevRSI.Rsi)
                    || (item.Value > prev.Value && itemRSI.Rsi <= prevRSI.Rsi))
                    {
                        Console.WriteLine($"SELL: {item.Date.ToString("dd/MM/yyyy HH")}");
                        return 1;
                    }
                }
            }
            var lBot = lTopBottom.Where(x => x.IsBot);
            if (lBot.Count() >= 2)
            {
                if (lTopBottom.SkipLast(2).Last().IsBot)
                {
                    var item = lBot.Last();
                    var prev = lBot.SkipLast(1).Last();
                    var itemRSI = lRsi.First(x => x.Date == item.Date);
                    var prevRSI = lRsi.First(x => x.Date == prev.Date);

                    if ((item.Value <= prev.Value && itemRSI.Rsi > prevRSI.Rsi)
                        || (item.Value < prev.Value && itemRSI.Rsi >= prevRSI.Rsi))
                    {
                        Console.WriteLine($"BUY: {item.Date.ToString("dd/MM/yyyy HH")}");
                        return 2;
                    }
                }
            }
            return 0;
        }
    }

    public static class clsEliot
    {
        public static (bool, int) CheckEliot(this List<Quote> lVal)
        {
            try
            {
                if (lVal.Count() < 100)
                    return (false, 0);

                var lTopBottom = lVal.GetTopBottomClean();
                var lTop = lTopBottom.Where(x => x.IsTop);
                var lBot = lTopBottom.Where(x => x.IsBot);
                var Top = lTop.Last();
                var NearTop = lTop.SkipLast(1).Last();
                var Bot = lBot.Last();
                var NearBot = lBot.SkipLast(1).Last();
                var cur = lVal.Last();
                var near = lVal.SkipLast(1).Last();
                if (cur.Close <= cur.Open
                    && cur.Close <= near.Close)
                    return (false, 0);

                var botDiv = Bot.Value - NearBot.Value;
                if(botDiv <= 0)
                    return (false, 0);

                var timeDiv = (Bot.Date - Top.Date).TotalMinutes;
                if(timeDiv > 0)
                {
                    
                    if(cur.Close > Top.Value
                        && near.Close <= Top.Value)
                    {
                        return (true, 1);
                    }
                }
                else
                {
                    var topDiv = Top.Value - NearTop.Value;
                    if (topDiv <= 0)
                        return (false, 0);

                    if (cur.Close > NearTop.Value)
                        return (true, 2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PartternService.CheckEliot|EXCEPTION| {ex.Message}");
            }
            return (false, 0); 
        }

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
                var lTopBottom = lVal.GetTopBottomClean_HL();
                var lTop = lTopBottom.Where(x => x.IsTop);
                var lBot = lTopBottom.Where(x => x.IsBot);
                var TopBottom = lTopBottom.Last();
                if (TopBottom.IsBot)
                {
                    var Top = lTop.Last();
                    var eMax = lVal.TakeLast(100).MaxBy(x => x.High);
                    var div = (Top.Date - eMax.Date).TotalMinutes;
                    if (div < 0)
                        return (false, 0);
                    var itemTop = lVal.First(x => x.Date == Top.Date);
                    var itemMax = lVal.First(x => x.Date == eMax.Date);
                    var indexTop = lVal.IndexOf(itemTop);
                    var indexMax = lVal.IndexOf(itemMax);
                    var c = indexTop - indexMax;
                    var x0 = (count - 1) - indexMax;

                    var val = (eMax.High - Top.Value) * x0 + c * (cur.Close - eMax.High);
                    if(val <= 0)
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
                    var eMin = lVal.TakeLast(100).MinBy(x => x.Low);
                    var div = (Bot.Date - eMin.Date).TotalMinutes;
                    if (div < 0)
                        return (false, 0);
                    var itemBot = lVal.First(x => x.Date == Bot.Date);
                    var itemMin = lVal.First(x => x.Date == eMin.Date);
                    var indexBot = lVal.IndexOf(itemBot);
                    var indexMin = lVal.IndexOf(itemMin);
                    var c = indexBot - indexMin;
                    var x0 = (count - 1) - indexMin;

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

        public static (bool, int) CheckPhanKy(this List<Quote> lVal)
        {
            try
            {
                if (lVal.Count() < 100)
                    return (false, 0);

                var lTopBottom = lVal.GetTopBottomClean();
                var lTop = lTopBottom.Where(x => x.IsTop);
                var lBot = lTopBottom.Where(x => x.IsBot);
                var Top = lTop.Last();
                var NearTop = lTop.SkipLast(1).Last();
                var Bot = lBot.Last();
                var NearBot = lBot.SkipLast(1).Last();
                var cur = lVal.Last();
                var near = lVal.SkipLast(1).Last();
                if (cur.Close <= cur.Open
                    && cur.Close <= near.Close)
                    return (false, 0);

                var botDiv = Bot.Value - NearBot.Value;
                if (botDiv <= 0)
                    return (false, 0);

                var timeDiv = (Bot.Date - Top.Date).TotalMinutes;
                if (timeDiv > 0)
                {

                    if (cur.Close > Top.Value
                        && near.Close <= Top.Value)
                    {
                        return (true, 1);
                    }
                }
                else
                {
                    var topDiv = Top.Value - NearTop.Value;
                    if (topDiv <= 0)
                        return (false, 0);

                    if (cur.Close > NearTop.Value)
                        return (true, 2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PartternService.CheckEliot|EXCEPTION| {ex.Message}");
            }
            return (false, 0);
        }
    }
}
