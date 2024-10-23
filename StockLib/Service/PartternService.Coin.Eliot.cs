using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyCoinEliot(string code)
        {
            try
            {
                _code = code;
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
                
                var lData = await _apiService.GetCoinData_Binance(code, "1w", 0);

                await SurveyCoinEliot(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyEliot|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SurveyCoinEliot(List<Quote> lData)
        {
            try
            {
                var lCheck = new List<Quote>();
                int index = 0;
                int indexBuy = 0;
                var lFracSignal = new List<FractalResult>();
                var flag = false;
                var buy = false;
                var lTmp = lData.GetFractal();
                FractalResult top = null;
                Quote cur = null;
                foreach (Quote l in lData)
                {
                    index++;
                    lCheck.Add(l);
                    if (flag && !buy)
                    {
                        if(l.GetRateCandleStick() >= 25)
                        {
                            if (l.Close > top.FractalBear)
                            {
                                cur = l;
                                buy = true;
                                indexBuy = index;
                                continue;
                            }
                        }
                    }

                    var lFrac = lCheck.GetFractal();
                    var lastFrac = lFrac?.SkipLast(2)?.LastOrDefault();
                    if (lastFrac != null 
                        && (lastFrac.FractalBear != null || lastFrac.FractalBull != null))
                    {
                        if(buy)
                        {
                            var rate = Math.Round(100 * (-1 + l.Close / cur.Close), 1);
                            _logger.LogInformation($"BUY: {cur.Date.ToString("dd/MM/yyyy")}|SELL: {l.Date.ToString("dd/MM/yyyy")}|Giu: {index - indexBuy} nen| Rate: {rate}%");  
                        }

                        flag = false;
                        buy = false;
                        top = null;
                        cur = null;
                        lFracSignal.Add(lastFrac);
                    }

                    if (!flag) 
                    {
                        var lastSignal = lFracSignal?.LastOrDefault();
                        var nearSignal = lFracSignal?.SkipLast(1).LastOrDefault();
                        if (lastSignal != null && nearSignal != null)
                        {
                            //if (lastSignal.FractalBear != null && nearSignal.FractalBear is null)
                            //{
                            //    //LH - Short
                            //    flag = true;
                            //    cur = l;
                            //}

                            if (lastSignal.FractalBull != null 
                                && nearSignal.FractalBull is null
                                && nearSignal.FractalBear > lastSignal.FractalBull)
                            {
                                //HL - Long
                                flag = true;
                                top = nearSignal;
                            }
                        }
                    }
                }

                var tmp = 1;
                //decimal low = 0, high = 0, div = 0;
                //foreach (var item in lFracResult)
                //{
                //    if(item.FractalBull != null)
                //    {
                //        if(div > 0)
                //        {
                //            var rate = Math.Round((high - item.FractalBull.Value) / div, 1);
                //            if(rate >= 0.3M && rate <= 0.6M)
                //            {
                //                Console.WriteLine($"{item.Date.ToString("dd/MM/yyyy")}");
                //            }    
                //        }

                //        low = item.FractalBull.Value;
                //    }
                //    if(item.FractalBear != null)
                //    {
                //        high = item.FractalBear.Value;
                //        div = high - low;
                //    }
                //}


                var isStart = false;
                var i = -1;
                //foreach (var item in lSuperTrend)
                //{
                //    i++;
                //    var itemData = lData.ElementAt(i);
                //    if (!isStart)
                //    {
                //        if (item.UpperBand != null)
                //        {
                //            isStart = true;
                //        }
                //        continue;
                //    }

                //    if (!_flagBuy)
                //    {
                //        if (item.LowerBand != null)
                //        {
                //            PrintBuy(itemData, i, true);
                //            _flagBuy = true;
                //        }
                //        continue;
                //    }

                //    if (_flagBuy)
                //    {
                //        if (item.UpperBand != null)
                //        {
                //            PrintBuy(itemData, i, false);
                //            _flagBuy = false;
                //        }
                //    }
                //}
                //PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveySuperTrend|EXCEPTION| {ex.Message}");
            }
        }
    }
}
