using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;
using System.Runtime.ConstrainedExecution;
using System;
using System.Net.WebSockets;

namespace StockLib.Service
{
    public partial class PartternService
    {
        private const int _interval = 5;
        public async Task SurveyCoinEliot(string code)
        {
            try
            {
                var lSymbols = await StaticVal.ByBitInstance().SpotApiV3.ExchangeData.GetSymbolsAsync();
                var lSymbolFilter = lSymbols.Data.Where(x => x.QuoteAsset == "USDT");
                var tm3p = lSymbolFilter.Count();
                foreach (var item in lSymbols.Data)
                {
                    _code = item.Alias;
                    //var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, code, Bybit.Net.Enums.KlineInterval.OneWeek, null, null, 1000);
                    //var lData = lByBit.Data.List.Select(x => new Quote
                    //{
                    //    Date = x.StartTime,
                    //    Open = x.OpenPrice,
                    //    High = x.HighPrice,
                    //    Low = x.LowPrice,
                    //    Close = x.ClosePrice,
                    //    Volume = x.Volume,
                    //}).ToList();
                    //lData.Reverse();

                    var lSymbol = await StaticVal.ByBitInstance().SpotApiV3.ExchangeData.GetSymbolsAsync();
                    var time = DateTimeOffset.Now.AddDays(-3).ToUnixTimeMilliseconds();

                    var lData = await _apiService.GetCoinData_Binance(_code, $"{_interval}m", time);
                    Thread.Sleep(200);
                    //await SurveyCoinEliot(lData);
                }

                var tmp = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyEliot|EXCEPTION| {ex.Message}");
            }
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
    }
}
