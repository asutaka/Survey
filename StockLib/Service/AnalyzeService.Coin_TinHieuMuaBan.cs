using CryptoExchange.Net.CommonObjects;
using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private long _time = DateTimeOffset.Now.AddDays(-30).ToUnixTimeMilliseconds();
        //private long _time = DateTimeOffset.Now.AddDays(-150).ToUnixTimeMilliseconds();
        private List<(int, string)> _lLong = new List<(int, string)>();
        public async Task<(int, string)> TinHieuMuaBanCoin_Binance()
        {
            try
            {
                var lSymbols = await _apiService.GetBinanceSymbol();
                foreach (var item in lSymbols)
                {
                    var coin = $"{item.FromAsset}{item.ToAsset}";
                    var lData = await _apiService.GetCoinData_Binance(coin, "1h", _time);
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

        public async Task<(int, string)> TinHieuMuaBanCoin_Bybit()
        {
            try
            {
                var lSymbols = await StaticVal.ByBitInstance().SpotApiV3.ExchangeData.GetSymbolsAsync();
                var lSymbolFilter = lSymbols.Data.Where(x => x.QuoteAsset == "USDT").Select(x => x.Alias);
                foreach (var item in lSymbolFilter)
                {
                    var res = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, item, Bybit.Net.Enums.KlineInterval.OneHour, null, null, 1000);
                    if (!res.Success)
                        return (0, null);

                    var lData = res.Data.List.Select(x => new Quote
                    {
                        Date = x.StartTime,
                        Open = x.OpenPrice,
                        High = x.HighPrice,
                        Low = x.LowPrice,
                        Close = x.ClosePrice,
                        Volume = x.Volume
                    }).ToList();


                    Thread.Sleep(200);
                    var eliot = lData.CheckEliot();
                    if (eliot.Item1)
                    {
                        _lLong.Add((eliot.Item2, item));
                    }
                }

                var sBuilder = new StringBuilder();


                if (_lLong.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Eliot sóng 3 - Bybit]");

                    var lBuy1 = _lLong.Where(x => x.Item1 == 1);
                    var lBuy2 = _lLong.Where(x => x.Item1 == 2);
                    if (lBuy1.Any())
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
