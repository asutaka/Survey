using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Model;
using StockLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private LinkedList<decimal> _lEth = new LinkedList<decimal>();
        private LinkedList<decimal> _lBtc = new LinkedList<decimal>();
        private LinkedList<decimal> _lXbt = new LinkedList<decimal>();
        private LinkedList<decimal> _lWld = new LinkedList<decimal>();
        private int N = 10;
        private void Add_Eth(decimal item)
        {
            if (_lEth.Count >= N)
                _lEth.RemoveLast();
            _lEth.AddFirst(item);
        }
        private void Add_Btc(decimal item)
        {
            if (_lBtc.Count >= N)
                _lBtc.RemoveLast();
            _lBtc.AddFirst(item);
        }
        private void Add_Xbt(decimal item)
        {
            if (_lXbt.Count >= N)
                _lXbt.RemoveLast();
            _lXbt.AddFirst(item);
        }
        private void Add_Wld(decimal item)
        {
            if (_lWld.Count >= N)
                _lWld.RemoveLast();
            _lWld.AddFirst(item);
        }

        public async Task SubcribeCoin()
        {
            try
            {
                var eth = StaticVal.BybitSocketInstance().V5LinearApi.SubscribeToTickerUpdatesAsync("ETHUSDT", (update) =>
                {
                    var lastPrice = update.Data.LastPrice;
                    if (lastPrice != null && lastPrice > 0)
                    {
                        Add_Eth(lastPrice ?? 0);
                        Console.WriteLine(_lEth.Count());
                        //Console.WriteLine($"ETH: {lastPrice}");
                    }
                });

                var btc = StaticVal.BybitSocketInstance().V5LinearApi.SubscribeToTickerUpdatesAsync("BTCUSDT", (update) =>
                {
                    var lastPrice = update.Data.LastPrice;
                    if (lastPrice != null && lastPrice > 0)
                    {
                        Add_Btc(lastPrice ?? 0);
                        //Console.WriteLine($"BTC: {lastPrice}");
                    }
                });

                var aixbt = StaticVal.BybitSocketInstance().V5LinearApi.SubscribeToTickerUpdatesAsync("AIXBTUSDT", (update) =>
                {
                    var lastPrice = update.Data.LastPrice;
                    if (lastPrice != null && lastPrice > 0)
                    {
                        Add_Xbt(lastPrice ?? 0);
                        //Console.WriteLine($"AIXBT: {lastPrice}");
                    }
                });

                var wld = StaticVal.BybitSocketInstance().V5LinearApi.SubscribeToTickerUpdatesAsync("WLDUSDT", (update) =>
                {
                    var lastPrice = update.Data.LastPrice;
                    if (lastPrice != null && lastPrice > 0)
                    {
                        Add_Wld(lastPrice ?? 0);
                        //Console.WriteLine($"WLD: {lastPrice}");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.SubcribeCoin|EXCEPTION| {ex.Message}");
            }
        }

        public async Task AnalyzeCoin(long userId, string input)
        {
            return;
            //var output = new StringBuilder();
            //if (string.IsNullOrWhiteSpace(input))
            //{
            //    return;
            //}
            //try
            //{
            //    //var socketClient = new BinanceSocketClient();
            //    //var tickerSubscriptionResult = socketClient.SpotApi.ExchangeData.SubscribeToTickerUpdatesAsync("ETHUSDT", (update) =>
            //    //{
            //    //    var lastPrice = update.Data.LastPrice;
            //    //});
            //    //if ("off".Equals(input.Trim(), StringComparison.OrdinalIgnoreCase))
            //    //{
            //    //    var res = StaticVal.BybitSocketInstance().V5SpotApi.SubscribeToTickerUpdatesAsync("ETHUSDT", (update) =>
            //    //    {
            //    //        var lastPrice = update.Data.LastPrice;
            //    //        Console.WriteLine(lastPrice);
            //    //    });
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            //input = input.RemoveSpace().ToUpper();
            //if ((input.StartsWith("[") && input.EndsWith("]"))
            //    || (input.StartsWith("*") && input.EndsWith("*"))
            //    || (input.StartsWith("@") && input.EndsWith("@")))//Nhóm ngành
            //{
            //    input = input.Replace("[", "").Replace("]", "").Replace("*", "").Replace("@", "");
            //    if (StaticVal._lBanLeKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Bán Lẻ
            //    {
            //        await PrintImage(EStockType.BanLe.ToString(), userId, true);
            //        await NganhBanLe(userId);
            //        return;
            //    }
            //    if (StaticVal._lBatDongSanKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Bất động sản
            //    {
            //        await PrintImage(EStockType.BDS.ToString(), userId, true);
            //        await NganhBatDongSan(userId);
            //        return;
            //    }
            //}
        }

        public async Task CalculateCoin()
        {
            try
            {
                var btc = "BTCUSDT";
                var btc4h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, btc, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 200);
                var lbtc4h = btc4h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lbtc4h.Reverse();
                var lOB_btc4h = _partternService.OrderBlock(lbtc4h?? new List<Quote>());
                Thread.Sleep(100);
                var btc1h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, btc, Bybit.Net.Enums.KlineInterval.OneHour, null, null, 200);
                var lbtc1h = btc1h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lbtc1h.Reverse();
                var lOB_btc1h = _partternService.OrderBlock(lbtc1h ?? new List<Quote>());
                Thread.Sleep(100);
                var btc15m = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, btc, Bybit.Net.Enums.KlineInterval.FifteenMinutes, null, null, 200);
                var lbtc15m = btc15m.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lbtc15m.Reverse();
                var lOB_btc15m = _partternService.OrderBlock(lbtc15m ?? new List<Quote>());
                Thread.Sleep(100);

                var eth = "ETHUSDT";
                var eth4h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, eth, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 200);
                var leth4h = eth4h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                leth4h.Reverse();
                var lOB_eth4h = _partternService.OrderBlock(leth4h ?? new List<Quote>());
                Thread.Sleep(100);

                var eth1h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, eth, Bybit.Net.Enums.KlineInterval.OneHour, null, null, 200);
                var leth1h = eth1h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                leth1h.Reverse();
                var lOB_eth1h = _partternService.OrderBlock(leth1h ?? new List<Quote>());
                Thread.Sleep(100);

                var eth15m = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, eth, Bybit.Net.Enums.KlineInterval.FifteenMinutes, null, null, 200);
                var leth15m = eth15m.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                leth15m.Reverse();
                var lOB_eth15m = _partternService.OrderBlock(leth15m ?? new List<Quote>());
                Thread.Sleep(100);

                var wld = "WLDUSDT";
                var wld4h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, wld, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 200);
                var lwld4h = wld4h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lwld4h.Reverse();
                var lOB_wld4h = _partternService.OrderBlock(lwld4h ?? new List<Quote>());
                Thread.Sleep(100);

                var wld1h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, wld, Bybit.Net.Enums.KlineInterval.OneHour, null, null, 200);
                var lwld1h = wld1h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lwld1h.Reverse();
                var lOB_wld1h = _partternService.OrderBlock(lwld1h ?? new List<Quote>());
                Thread.Sleep(100);

                var wld15m = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, wld, Bybit.Net.Enums.KlineInterval.FifteenMinutes, null, null, 200);
                var lwld15m = wld15m.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lwld15m.Reverse();
                var lOB_wld15m = _partternService.OrderBlock(lwld15m ?? new List<Quote>());
                Thread.Sleep(100);

                var aixbt = "AIXBTUSDT";
                var aixbt4h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, aixbt, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 200);
                var laixbt4h = aixbt4h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                laixbt4h.Reverse();
                var lOB_aixbt4h = _partternService.OrderBlock(laixbt4h ?? new List<Quote>());
                Thread.Sleep(100);
                var aixbt1h = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, aixbt, Bybit.Net.Enums.KlineInterval.OneHour, null, null, 200);
                var laixbt1h = aixbt1h.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                laixbt1h.Reverse();
                var lOB_aixbt1h = _partternService.OrderBlock(laixbt1h ?? new List<Quote>());
                Thread.Sleep(100);
                var aixbt15m = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Inverse, aixbt, Bybit.Net.Enums.KlineInterval.FifteenMinutes, null, null, 200);
                var laixbt15m = aixbt15m.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                laixbt15m.Reverse();
                var lOB_aixbt15m = _partternService.OrderBlock(laixbt15m ?? new List<Quote>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.CalculateCoin|EXCEPTION| {ex.Message}");
            }
        }
    }
}
