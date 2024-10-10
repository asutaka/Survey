using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyCoinSuperTrend(string code)
        {
            try
            {
                _code = code;
                var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, code, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 1000);
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

                await SurveyCoinSuperTrend(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyCoinSuperTrend|EXCEPTION| {ex.Message}");
            }
        }
        private async Task SurveyCoinSuperTrend(List<Quote> lData)
        {
            try
            {
                var lSuperTrend = lData.GetSuperTrend();
                var isStart = false;
                var i = -1;
                foreach (var item in lSuperTrend)
                {
                    i++;
                    var itemData = lData.ElementAt(i);
                    if (!isStart)
                    {
                        if(item.UpperBand != null)
                        {
                            isStart = true;
                        }
                        continue;
                    }

                    if(!_flagBuy)
                    {
                        if(item.LowerBand != null)
                        {
                            PrintBuy(itemData, i, true);
                            _flagBuy = true;
                        }
                        continue;
                    }

                    if(_flagBuy)
                    {
                        if(item.UpperBand != null)
                        {
                            PrintBuy(itemData, i, false);
                            _flagBuy = false;
                        }
                    }
                }
                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveySuperTrend|EXCEPTION| {ex.Message}");
            }
        }

        public async Task SurveyCoinSuperTrendPhrase2(string code)
        {
            try
            {
                _code = code;

                var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, code, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 500);
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
                await SurveyCoinSuperTrendPhrase2(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyCoinSuperTrendPhrase2|EXCEPTION| {ex.Message}");
            }
        }
        private async Task SurveyCoinSuperTrendPhrase2(List<Quote> lData)
        {
            try
            {
                var lSuperTrend = lData.GetSuperTrend();
                var isStart = false;
                var i = -1;
                foreach (var item in lSuperTrend)
                {
                    i++;
                    
                    if (!isStart)
                    {
                        if (item.UpperBand != null)
                        {
                            isStart = true;
                        }
                        continue;
                    }

                    if (i < 10)
                        continue;

                    var cur = lData.ElementAt(i);

                    if (!_flagBuy)
                    {
                        var near = lSuperTrend.ElementAt(i - 1);
                        var near2 = lSuperTrend.ElementAt(i - 2);
                        var near3 = lSuperTrend.ElementAt(i - 3);
                        var near4 = lSuperTrend.ElementAt(i - 4);
                        var near5 = lSuperTrend.ElementAt(i - 5);
                        if (near.LowerBand is null
                            || near2.LowerBand is null
                            || near3.LowerBand is null
                            || near4.LowerBand is null
                            || near5.LowerBand is null)
                            continue;

                        if (near2.LowerBand != near3.LowerBand
                            || near2.LowerBand != near4.LowerBand
                            || near2.LowerBand != near5.LowerBand)
                            continue;

                        if (near.LowerBand > near2.LowerBand
                            || (near.LowerBand == near2.LowerBand && item.LowerBand > near.LowerBand))
                        {
                            if (cur.Close <= cur.Open)
                                continue;

                            PrintBuy(cur, i, true);
                            _flagBuy = true;
                        }
                        continue;
                    }

                    if (_flagBuy)
                    {
                        if (item.UpperBand != null)
                        {
                            PrintBuy(cur, i, false);
                            _flagBuy = false;
                        }
                    }
                }
                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyCoinSuperTrendPhrase2|EXCEPTION| {ex.Message}");
            }
        }
    }
}
