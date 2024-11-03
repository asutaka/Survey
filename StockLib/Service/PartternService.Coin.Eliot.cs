using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;
using System.Runtime.ConstrainedExecution;
using System;
using System.Net.WebSockets;
using System.Xml.Linq;

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
                for ( int j = 1; j < count - 1; j++)
                {
                    var lData = lAll.Take(j).ToList();
                    await CheckPhanKy(lData );
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
            var lBB = lData.GetBollingerBands();
            var lTopBottom = lData.GetTopBottomClean();
            var lTop = lTopBottom.Where(x => x.IsTop);
            if (lTop.Count() >= 2)
            {
                if (lTopBottom.ElementAt(j - 2).IsTop)
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
                if (lTopBottom.ElementAt(j - 2).IsBot)
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
