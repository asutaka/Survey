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

                    var lData = await _apiService.GetCoinData_Binance(_code, "5m", time);
                    Thread.Sleep(200);
                    await Test(lData, _code);
                    //await SurveyCoinEliot(lData);
                }

                var tmp = 1;
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
                    if (l.Close > top.FractalBear)
                    {
                        cur = l;
                        buy = true;
                        indexBuy = index;
                        continue;
                    }
                    //if (l.GetRateCandleStick() >= 25)
                    //{

                    //}
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

        private List<string> _lCoin = new List<string>();
        private bool _flag = false;
        private async Task Test(List<Quote> lData, string coin)
        {
            if (lData.Count() < 100)
                return;

            var test = lData.GetTopBottom();
            var test2 = lData.GetTopBottomClean();

            var lema34 = lData.GetEma(34);
            var lema89 = lData.GetEma(89);
            //var lFrac = lData.GetFractal();
            //var lMode = new List<(FractalResult, int)>();
            var count = lema34.Count();
            var isMin = true;
            Quote minObj = null;
            for (int i = 30; i < count - 2; i++) 
            {
                var item = lData.ElementAt(i);
                if(!isMin)
                {
                    for (int j = 0; j < i; j++)
                    {
                        var itemPrev = lData.ElementAt(j);
                        if (item.Low > itemPrev.Low)
                        {
                            isMin = false;
                            break;
                        }
                    }

                    if (!isMin)
                        continue;

                    var itemNext = lData.ElementAt(i + 1);
                    var itemNext2 = lData.ElementAt(i + 2);
                    if (itemNext.Low < item.Low
                        || itemNext2.Low < item.Low)
                    {
                        isMin = false;
                        continue;
                    }

                    minObj = item;
                    i = i + 2;
                    continue;
                }

                if(item.Low < minObj?.Low)
                {
                    minObj = item;
                    continue;
                }

                var ema34 = lema34.ElementAt(i);
                var ema89 = lema89.ElementAt(i);
                if (item.Close < item.Open
                    || item.High <= (decimal)Math.Max((ema34.Ema ?? 0), (ema89.Ema ?? 0))) 
                {
                    continue;
                }

                Console.WriteLine($"{item.Date.ToString("dd/MM/yyyy HH:mm:ss")}");
            }

            var x = 1;

            //var ema34_0 = lema34.Last();
            //var ema34_1 = lema34.SkipLast(1).Last();
            //var ema34_2 = lema34.SkipLast(2).Last();
            //var ema34_3 = lema34.SkipLast(3).Last();
            //var ema34_4 = lema34.SkipLast(4).Last();
            //var ema34_5 = lema34.SkipLast(5).Last();
            //var ema34_6 = lema34.SkipLast(6).Last();

            //var ema89_0 = lema89.Last();
            //var ema89_1 = lema89.SkipLast(1).Last();
            //var ema89_2 = lema89.SkipLast(2).Last();
            //var ema89_3 = lema89.SkipLast(3).Last();
            //var ema89_4 = lema89.SkipLast(4).Last();
            //var ema89_5 = lema89.SkipLast(5).Last();
            //var ema89_6 = lema89.SkipLast(6).Last();

            //if(ema34_0.Ema > ema89_0.Ema)
            //{
            //    var tmp = 1;
            //}


            //if ((ema34_0.Ema > ema89_0.Ema && ema34_1.Ema <= ema89_1.Ema)
            //    || (ema34_1.Ema > ema89_1.Ema && ema34_2.Ema <= ema89_2.Ema)
            //    || (ema34_2.Ema > ema89_2.Ema && ema34_3.Ema <= ema89_3.Ema)
            //    || (ema34_3.Ema > ema89_3.Ema && ema34_4.Ema <= ema89_4.Ema)
            //    || (ema34_4.Ema > ema89_4.Ema && ema34_5.Ema <= ema89_5.Ema)
            //    || (ema34_5.Ema > ema89_5.Ema && ema34_6.Ema <= ema89_6.Ema))
            //{
            //    _lCoin.Add(coin);
            //}
        }

        //private async Task Test(List<Quote> lData, string coin)
        //{
        //    if (lData.Count() < 100)
        //        return;

        //    var lema34 = lData.GetEma(34);
        //    var lema89 = lData.GetEma(89);
        //    var lFrac = lData.GetFractal();
        //    var lMode = new List<(FractalResult, int)>();
        //    var count = lema34.Count();
        //    for (int i = 1; i < count; i++)
        //    {
        //        var itemFrac = lFrac.ElementAt(i);
        //        if (itemFrac.FractalBear != null
        //           || itemFrac.FractalBull != null)
        //            lMode.Add((itemFrac, i));

        //        if (lMode.Count() < 4)
        //        {
        //            _flag = false;
        //            continue;
        //        }

        //        var lastMode = lMode.Last();
        //        if (lastMode.Item1.FractalBull is null)
        //        {
        //            _flag = false;
        //            continue;
        //        }
        //        var nearMode = lMode.SkipLast(1).Last();
        //        if (nearMode.Item1.FractalBear is null)
        //        {
        //            _flag = false;
        //            continue;
        //        }

        //        if (lastMode.Item1.FractalBull >= nearMode.Item1.FractalBear)
        //        {
        //            _flag = false;
        //            continue;
        //        }

        //        var nearNextMode = lMode.SkipLast(2).Last();
        //        var nearNextNextMode = lMode.SkipLast(3).Last();
        //        if (nearNextMode.Item1.FractalBull != null && lastMode.Item1.FractalBull > nearNextMode.Item1.FractalBull)
        //        {
        //            _flag = false;
        //            continue;
        //        }
        //        else if (nearNextNextMode.Item1.FractalBull != null && lastMode.Item1.FractalBull > nearNextNextMode.Item1.FractalBull)
        //        {
        //            _flag = false;
        //            continue;
        //        }

        //        //if(lastMode.Item2 - nearMode.Item2 < 5)
        //        //{
        //        //    _flag = false;
        //        //    continue;
        //        //}

        //        var item34 = lema34.ElementAt(i);
        //        var item89 = lema89.ElementAt(i);
        //        var item = lData.ElementAt(i);
        //        var itemNear = lData.ElementAt(i - 1);
        //        var max = Math.Max((item34.Ema ?? 0), (item89.Ema ?? 0));

        //        if (item.Close > item.Open
        //            && (double)item.High > max)
        //        {
        //            _flag = true;
        //            //Console.WriteLine($"{item.Date.ToString("dd/MM/yyyy HH:mm:ss")}: {item.Close}");
        //        }

        //        if (_flag)
        //        {
        //            if (item.High > nearMode.Item1.FractalBear
        //                && itemNear.High <= nearMode.Item1.FractalBear)
        //            {
        //                Console.WriteLine($"{item.Date.ToString("dd/MM/yyyy HH:mm:ss")}|Bear: {lastMode.Item1.Date.ToString("dd/MM/yyyy HH:mm:ss")}|Bull: {nearMode.Item1.Date.ToString("dd/MM/yyyy HH:mm:ss")}");
        //                _flag = false;
        //            }
        //        }


        //        //var item34_Prev = lema34.ElementAt(i-1);
        //        //var item89_Prev = lema89.ElementAt(i-1);
        //        //if (item.Close > item.Open
        //        //    && item34.Ema > item89.Ema 
        //        //    && item34_Prev.Ema <= item89_Prev.Ema)
        //        //{
        //        //    var tmp = 1;
        //        //}
        //    }

        //    var x = 1;

        //    //var ema34_0 = lema34.Last();
        //    //var ema34_1 = lema34.SkipLast(1).Last();
        //    //var ema34_2 = lema34.SkipLast(2).Last();
        //    //var ema34_3 = lema34.SkipLast(3).Last();
        //    //var ema34_4 = lema34.SkipLast(4).Last();
        //    //var ema34_5 = lema34.SkipLast(5).Last();
        //    //var ema34_6 = lema34.SkipLast(6).Last();

        //    //var ema89_0 = lema89.Last();
        //    //var ema89_1 = lema89.SkipLast(1).Last();
        //    //var ema89_2 = lema89.SkipLast(2).Last();
        //    //var ema89_3 = lema89.SkipLast(3).Last();
        //    //var ema89_4 = lema89.SkipLast(4).Last();
        //    //var ema89_5 = lema89.SkipLast(5).Last();
        //    //var ema89_6 = lema89.SkipLast(6).Last();

        //    //if(ema34_0.Ema > ema89_0.Ema)
        //    //{
        //    //    var tmp = 1;
        //    //}


        //    //if ((ema34_0.Ema > ema89_0.Ema && ema34_1.Ema <= ema89_1.Ema)
        //    //    || (ema34_1.Ema > ema89_1.Ema && ema34_2.Ema <= ema89_2.Ema)
        //    //    || (ema34_2.Ema > ema89_2.Ema && ema34_3.Ema <= ema89_3.Ema)
        //    //    || (ema34_3.Ema > ema89_3.Ema && ema34_4.Ema <= ema89_4.Ema)
        //    //    || (ema34_4.Ema > ema89_4.Ema && ema34_5.Ema <= ema89_5.Ema)
        //    //    || (ema34_5.Ema > ema89_5.Ema && ema34_6.Ema <= ema89_6.Ema))
        //    //{
        //    //    _lCoin.Add(coin);
        //    //}
        //}
    }
}
