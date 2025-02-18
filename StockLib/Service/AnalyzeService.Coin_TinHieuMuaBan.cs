﻿using Binance.Net.Clients;
using CryptoExchange.Net.Interfaces;
using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private long _time = DateTimeOffset.Now.AddDays(-150).ToUnixTimeMilliseconds();
        private List<(int, string)> _lLong = new List<(int, string)>();
        private List<(int, string)> _lShort = new List<(int, string)>();

        private (int, string) RSICheck(double? rsi, Quote quote)
        {
            if (rsi.Value >= 75)
            {
                return (1, $"[BTC cung 15 phút quá mua mức 2] giá: {quote.Close}");
            }
            else if (rsi.Value >= 70)
            {
                return (1, $"[BTC cung 15 phút quá mua mức 1] giá: {quote.Close}");
            }
            return (0, null);
        }
        public async Task<(int, string)> CanhBaoBitcoin()
        {
            try
            {
                var l15m = await _apiService.GetCoinData_Binance("BTCUSDT", "15m", DateTimeOffset.Now.AddDays(-3).ToUnixTimeMilliseconds());
                var l15m_rsi = l15m.GetRsi(14);
                var rsi15m_last = l15m_rsi.Last();
                var res15m = RSICheck(rsi15m_last.Rsi, l15m.Last());
                if (res15m.Item1 > 0)
                    return res15m;

                var l1h = await _apiService.GetCoinData_Binance("BTCUSDT", "1h", DateTimeOffset.Now.AddDays(-10).ToUnixTimeMilliseconds());
                var l1h_rsi = l1h.GetRsi(14);
                var rsi1h_last = l1h_rsi.Last();
                var res1h = RSICheck(rsi1h_last.Rsi, l1h.Last());
                if (res1h.Item1 > 0)
                    return res1h;

                var l4h = await _apiService.GetCoinData_Binance("BTCUSDT", "4h", DateTimeOffset.Now.AddDays(-30).ToUnixTimeMilliseconds());
                var l4h_rsi = l4h.GetRsi(14);
                var rsi4h_last = l4h_rsi.Last();
                var res4h = RSICheck(rsi4h_last.Rsi, l4h.Last());
                if (res4h.Item1 > 0)
                    return res4h;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (0, null);
        }

        public async Task<(int, string)> TraceGiaCoin(string coin)
        {
            try
            {
                var l15m = await _apiService.GetCoinData_Binance(coin, "15m", DateTimeOffset.Now.AddDays(-1).ToUnixTimeMilliseconds());
                return (1, $"Giá {coin}: {l15m.Last().Close}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (0, null);
        }

        public async Task<(int, string)> TinHieuMuaBanCoin_Binance()
        {
            try
            {
                var lSymbols = await _apiService.GetBinanceSymbol();
                foreach (var item in lSymbols)
                {
                    var coin = $"{item.FromAsset}{item.ToAsset}";
                    var lData = await _apiService.GetCoinData_Binance(coin, "4h", _time);
                    Thread.Sleep(200);
                    var eliot = lData.CheckEliot();
                    if (eliot.Item1)
                    {
                        _lLong.Add((eliot.Item2, coin));
                    }
                }

                var sBuilder = new StringBuilder();


                if (_lLong.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Eliot sóng 3 - Binance]");

                    var lBuy1 = _lLong.Where(x => x.Item1 == 1);
                    var lBuy2 = _lLong.Where(x => x.Item1 == 2);
                    if(lBuy1.Any())
                    {
                        sBuilder.AppendLine("+ Mua vượt đỉnh:");
                        foreach (var item in lBuy1.Take(10))
                        {
                            sBuilder.AppendLine($"   - {item.Item2}");
                        }
                    }
                    if (lBuy2.Any())
                    {
                        sBuilder.AppendLine("+ Mua Test lại:");
                        foreach (var item in lBuy2.Take(15))
                        {
                            sBuilder.AppendLine($"   - {item.Item2}");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(sBuilder.ToString()))
                {
                    return (0, null);
                }
                return (1, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TinHieuMuaBanCoin_Binance|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        public async Task<(int, string)> TinHieuMuaBanCoin_PriceAction_Binance()
        {
            try
            {
                var lSymbols = await _apiService.GetBinanceSymbol();
                foreach (var item in lSymbols)
                {
                    var coin = $"{item.FromAsset}{item.ToAsset}";
                    var lData = await _apiService.GetCoinData_Binance(coin, "4h", _time);
                    Thread.Sleep(200);
                    var eliot = lData.CheckPriceAction();
                    if (eliot.Item1)
                    {
                        _lLong.Add((eliot.Item2, coin));
                    }
                }

                var sBuilder = new StringBuilder();


                if (_lLong.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Eliot sóng 3 - Binance]");

                    var lBuy = _lLong.Where(x => x.Item1 == 1);
                    var lSell = _lLong.Where(x => x.Item1 == 2);
                    if (lBuy.Any())
                    {
                        sBuilder.AppendLine("+ Mua:");
                        foreach (var item in lBuy.Take(10))
                        {
                            sBuilder.AppendLine($"   - {item.Item2}");
                        }
                    }
                    if (lSell.Any())
                    {
                        sBuilder.AppendLine("+ Bán:");
                        foreach (var item in lSell.Take(10))
                        {
                            sBuilder.AppendLine($"   - {item.Item2}");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(sBuilder.ToString()))
                {
                    return (0, null);
                }
                return (1, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TinHieuMuaBanCoin_Binance|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        private (bool, string) BatDay(List<Quote> lData)
        {
            try
            {
                var lVol = new List<Quote>();
                foreach (var item in lData)
                {
                    lVol.Add(new Quote { Date = item.Date, Close = item.Volume, Open = item.Open, High = item.High, Low = item.Low, Volume = item.Volume });
                }
                var lVolMa = lVol.GetSma(20);
                var lBB = lData.GetBollingerBands();

                var lastItem = lData.Last();
                var lastVol = lVolMa.Last();
                var lastBB = lBB.Last();

                var Trendline = lData.CheckTrendline(lVolMa, lBB);
                if (Trendline.Item1)
                {
                    return (true, "TrendLine");
                    //Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {Trendline.Item2}");
                }

                var BatDay = lastItem.CheckBatDay(lastVol, lastBB);
                if (BatDay.Item1)
                {
                    return (true, "TrendLine");
                    //Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {BatDay.Item2}");
                }

                lastItem = lData.SkipLast(1).Last();
                lastVol = lVolMa.SkipLast(1).Last();
                lastBB = lBB.SkipLast(1).Last();
                BatDay = lastItem.CheckBatDay(lastVol, lastBB);
                if (BatDay.Item1)
                {
                    return (true, "BatDay");
                    //Console.WriteLine($"{lastItem.Date.ToString($"dd/MM/yyyy HH")}|ENTRY: {BatDay.Item2}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.BatDay|EXCEPTION| {ex.Message}");
            }

            return (false, null);
        }

        public async Task<(int, string)> LiquidTrace()
        {
            await _socketService.LiquidWebSocket("wss://ws.coinank.com/wsKline/wsKline");
            return (0, null);
        }

        public async Task<(int, string)> TinHieuMuaBanCoin_Bybit()
        {
            try
            {
                //var socketClient = new BinanceSocketClient();
                //var tickerSubscriptionResult = socketClient.SpotApi.ExchangeData.SubscribeToTickerUpdatesAsync("ETHUSDT", (update) =>
                //{
                //    var lastPrice = update.Data.LastPrice;
                //});

                //var tickerSubscriptionResult = StaticVal.BybitSocketInstance().V5SpotApi.SubscribeToTickerUpdatesAsync("ETHUSDT", (update) =>
                //{
                //    var lastPrice = update.Data.LastPrice;
                //    Console.WriteLine(lastPrice);
                //});
                //return (0, null);

                var lSymbols = await _apiService.GetBybitSymbol();
                var lSymbolFilter = lSymbols.Select(x => x.si);
                var lRes = new List<string>();
                foreach (var item in lSymbolFilter)
                {
                    try
                    {
                        var res4h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, item, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 200);
                        if (!res4h.Success)
                            continue;

                        if (res4h.Data.List.Count() < 20)
                            continue;

                        var lData4h = res4h.Data.List.Select(x => new Quote
                        {
                            Date = x.StartTime,
                            Open = x.OpenPrice,
                            High = x.HighPrice,
                            Low = x.LowPrice,
                            Close = x.ClosePrice,
                            Volume = x.Volume
                        }).ToList();
                        lData4h.Reverse();
                        var check = BatDay(lData4h);
                        if (check.Item1)
                        {
                            lRes.Add($"{item}: {check.Item2}");
                            Thread.Sleep(200);
                            continue;
                        }

                        Thread.Sleep(200);

                        var res15m = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, item, Bybit.Net.Enums.KlineInterval.FifteenMinutes, null, null, 200);
                        if (!res15m.Success)
                            continue;

                        var lData15m = res15m.Data.List.Select(x => new Quote
                        {
                            Date = x.StartTime,
                            Open = x.OpenPrice,
                            High = x.HighPrice,
                            Low = x.LowPrice,
                            Close = x.ClosePrice,
                            Volume = x.Volume
                        }).ToList();
                        lData15m.Reverse();
                        check = BatDay(lData15m);
                        if (check.Item1)
                        {
                            lRes.Add($"{item}: {check.Item2}");
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Thread.Sleep(200);
                }

                var sBuilder = new StringBuilder();
                if (lRes.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[COIN]");
                    foreach (var item in lRes.Take(15))
                    {
                        sBuilder.AppendLine($"   - {item}");
                    }
                }

                if (string.IsNullOrWhiteSpace(sBuilder.ToString()))
                {
                    return (0, null);
                }
                return (1, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TinHieuMuaBanCoin_Bybit|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        //public async Task<(int, string)> TinHieuMuaBanCoin()
        //{
        //    try
        //    {
        //        var lCoin = _coinRepo.GetAll();

        //        var lDanZVolume = new List<Coin>();
        //        var lSuperTrend = new List<Coin>();
        //        var lSuperTrendPhrase2 = new List<Coin>();

        //        foreach (var item in lCoin)
        //        {
        //            if (item is null
        //                || !item.indicator.Any())
        //                continue;

        //            var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, item.s, Bybit.Net.Enums.KlineInterval.OneHour, null, null, 1000);
        //            var lData = lByBit.Data.List.Select(x => new Quote
        //            {
        //                Date = x.StartTime,
        //                Open = x.OpenPrice,
        //                High = x.HighPrice,
        //                Low = x.LowPrice,
        //                Close = x.ClosePrice,
        //                Volume = x.Volume,
        //            }).ToList();

        //            if (item.indicator.Any(x => x.ty == (int)EIndicator.DanZangerVolumne))
        //            {
        //                var isDanz = lData.CheckDanZangerCustom(7);
        //                if (isDanz)
        //                {
        //                    lDanZVolume.Add(item);
        //                }
        //            }

        //            if (item.indicator.Any(x => x.ty == (int)EIndicator.SuperTrend))
        //            {
        //                var isSuperTrend = lData.CheckSuperTrend();
        //                if (isSuperTrend)
        //                {
        //                    lSuperTrend.Add(item);
        //                }
        //            }


        //            if (item.indicator.Any(x => x.ty == (int)EIndicator.SuperTrendPhrase2))
        //            {
        //                var isSuperTrendPhrase2 = lData.CheckSuperTrendPharse2();
        //                if (isSuperTrendPhrase2)
        //                {
        //                    lSuperTrendPhrase2.Add(item);
        //                }
        //            }
        //        }

        //        var sBuilder = new StringBuilder();


        //        if (lDanZVolume.Any())
        //        {
        //            sBuilder.AppendLine();
        //            sBuilder.AppendLine("[Tín hiệu DanZVolume - COIN]");

        //            foreach (var item in lDanZVolume.Take(10))
        //            {
        //                var indicator = item.indicator.FirstOrDefault(x => x.ty == (int)EIndicator.SuperTrend);
        //                sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg}%|Nam giu: {indicator.num}| Win/Loss: {indicator.win}%/{indicator.loss}%)");
        //            }
        //        }

        //        if (lSuperTrend.Any())
        //        {
        //            sBuilder.AppendLine();
        //            sBuilder.AppendLine("[Tín hiệu SuperTrend - COIN]");

        //            foreach (var item in lSuperTrend.Take(10))
        //            {
        //                var indicator = item.indicator.FirstOrDefault(x => x.ty == (int)EIndicator.SuperTrend);
        //                sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg}%|Nam giu: {indicator.num}| Win/Loss: {indicator.win}%/{indicator.loss}%)");
        //            }
        //        }

        //        if (lSuperTrendPhrase2.Any())
        //        {
        //            sBuilder.AppendLine();
        //            sBuilder.AppendLine("[Tín hiệu SuperTrend - Phrase 2 - COIN]");

        //            foreach (var item in lSuperTrendPhrase2.Take(10))
        //            {
        //                var indicator = item.indicator.FirstOrDefault(x => x.ty == (int)EIndicator.SuperTrendPhrase2);
        //                sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg}%|Nam giu: {indicator.num}| Win/Loss: {indicator.win}%/{indicator.loss}%)");
        //            }
        //        }

        //        if (string.IsNullOrWhiteSpace(sBuilder.ToString()))
        //        {
        //            return (0, null);
        //        }
        //        return (1, sBuilder.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"AnalyzeService.TinHieuMuaBanCoin|EXCEPTION| {ex.Message}");
        //    }

        //    return (0, null);
        //}
    }
}
