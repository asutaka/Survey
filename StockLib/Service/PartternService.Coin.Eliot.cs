using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;
using System.Runtime.ConstrainedExecution;
using System;

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

        private List<(DateTime, DateTime, int, decimal)> lTotal = new List<(DateTime, DateTime, int, decimal)>();
        public void PrintAll()
        {
            foreach (var item in lTotal.OrderBy(x => x.Item1).ThenBy(x => x.Item2))
            {
                _logger.LogInformation($"BUY: {item.Item1.ToString("dd/MM/yyyy")}|SELL: {item.Item2.ToString("dd/MM/yyyy")}|Giu: {item.Item3} nen| Rate: {item.Item4}%");
            } 
        }
        private async Task LongVuotDinh(List<Quote> lData)
        {
            var lCheck = new List<Quote>();
            int index = 0;
            int indexBuy = 0;
            var lFracSignal = new List<FractalResult>();
            var flag = false;
            var buy = false;
            FractalResult top = null;
            Quote cur = null;
            foreach (Quote l in lData)
            {
                index++;
                lCheck.Add(l);
                if (flag && !buy)
                {
                    if (l.GetRateCandleStick() >= 25)
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
                    if (buy)
                    {
                        var rate = Math.Round(100 * (-1 + l.Close / cur.Close), 1);
                        //_logger.LogInformation($"BUY: {cur.Date.ToString("dd/MM/yyyy")}|SELL: {l.Date.ToString("dd/MM/yyyy")}|Giu: {index - indexBuy} nen| Rate: {rate}%");
                        lTotal.Add((cur.Date, l.Date, (index - indexBuy), rate));
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
        }

        private async Task LongVuotDinhThemFilter(List<Quote> lData)
        {
            var lCheck = new List<Quote>();
            int index = 0;
            int indexBuy = 0;
            var lFracSignal = new List<FractalResult>();
            var flag = false;
            var buy = false;
            decimal rateEliot = 0;
            FractalResult top = null;
            Quote cur = null;
            foreach (Quote l in lData)
            {
                index++;
                lCheck.Add(l);
                if (flag && !buy)
                {
                    if (l.GetRateCandleStick() >= 25)
                    {
                        //if (l.Close > top.FractalBear 
                            //&& rateEliot >= 30 && rateEliot <= 60)
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
                    if (buy)
                    {
                        var rate = Math.Round(100 * (-1 + l.Close / cur.Close), 1);
                        _logger.LogInformation($"BUY: {cur.Date.ToString("dd/MM/yyyy")}|SELL: {l.Date.ToString("dd/MM/yyyy")}|Giu: {index - indexBuy} nen| Eliot: {rateEliot}%| Rate: {rate}%");
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
                    var nearNext = lFracSignal?.SkipLast(2).LastOrDefault();
                    if (lastSignal != null && nearSignal != null)
                    {
                        if (lastSignal.FractalBull != null
                            && nearSignal.FractalBull is null
                            && nearSignal.FractalBear > lastSignal.FractalBull
                            && nearNext?.FractalBull != null) 
                        {
                            var up = nearSignal.FractalBear - nearNext.FractalBull;
                            var down = nearSignal.FractalBear - lastSignal.FractalBull;
                            rateEliot = Math.Round(100 * (down.Value / up.Value), 1);
                            //_logger.LogInformation($"INFO: {rateEliot}%");

                            //HL - Long
                            flag = true;
                            top = nearSignal;
                        }
                    }
                }
            }
        }

        private async Task ShortThungDay(List<Quote> lData)
        {
            var lCheck = new List<Quote>();
            int index = 0;
            int indexBuy = 0;
            var lFracSignal = new List<FractalResult>();
            var flag = false;
            var buy = false;
            FractalResult top = null;
            Quote cur = null;
            foreach (Quote l in lData)
            {
                index++;
                lCheck.Add(l);
                if (flag && !buy)
                {
                    if (l.GetRateCandleStick() >= 40)
                    {
                        //cur = l;
                        //buy = true;
                        //indexBuy = index;
                        //continue;
                        if (l.Close < top.FractalBull)
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
                    if (buy)
                    {
                        var rate = Math.Round(100 * (-1 + cur.Close / l.Close), 1);
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
                        if (lastSignal.FractalBear != null
                            && nearSignal.FractalBear is null
                            && lastSignal.FractalBear > nearSignal.FractalBull)
                        {
                            //LH - Short
                            flag = true;
                            top = nearSignal;
                        }
                    }
                }
            }
        }

        private async Task SurveyCoinEliot(List<Quote> lData)
        {
            try
            {
                await LongVuotDinh(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveySuperTrend|EXCEPTION| {ex.Message}");
            }
        }
    }
}
