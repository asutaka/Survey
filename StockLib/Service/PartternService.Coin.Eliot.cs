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
                foreach (Quote l in lData)
                {
                    index++;
                    lCheck.Add(l);
                    var lFrac = lCheck.GetFractal();
                    var lFracFilter = lFrac.Where(x => x.FractalBear != null || x.FractalBull != null);
                    var lFracResult = new List<FractalResult>();
                    foreach (var fractal in lFracFilter)
                    {
                        var last = lFracResult.LastOrDefault();
                        if (last != null)
                        {
                            if (last.FractalBear != null && fractal.FractalBear != null)
                            {
                                if (last.FractalBear < fractal.FractalBear)
                                {
                                    lFracResult.Remove(last);
                                    lFracResult.Add(fractal);
                                }
                                continue;
                            }
                            else if (last.FractalBull != null && fractal.FractalBull != null)
                            {
                                if (last.FractalBull > fractal.FractalBull)
                                {
                                    lFracResult.Remove(last);
                                    lFracResult.Add(fractal);
                                }
                                continue;
                            }
                        }

                        lFracResult.Add(fractal);
                    }

                    if(lFracResult.Any())
                    {
                        var last = lFracResult.Last();
                        var near = lFracResult.SkipLast(1).LastOrDefault();
                        if (near != null && (l.Date - near.Date).TotalDays / 7 < 3)
                        {
                            var isBull = near.FractalBull != null;
                            _logger.LogInformation($"Trace {index} {l.Date.ToShortDateString()}: {near.Date.ToShortDateString()}({(isBull ? "L" : "H")})");
                        }
                        if ((l.Date - last.Date).TotalDays/7 < 3)
                        {
                            var isBull = last.FractalBull != null;
                            _logger.LogInformation($"Trace {index} {l.Date.ToShortDateString()}: {last.Date.ToShortDateString()}({(isBull? "L" : "H")})");
                        }
                    }    
                }
                

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
