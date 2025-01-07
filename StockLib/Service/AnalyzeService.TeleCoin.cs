using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private static LinkedList<(decimal, DateTime)> _lEth = new LinkedList<(decimal, DateTime)>();
        private static LinkedList<(decimal, DateTime)> _lBtc = new LinkedList<(decimal, DateTime)>();
        private static LinkedList<(decimal, DateTime)> _lXbt = new LinkedList<(decimal, DateTime)>();
        private static LinkedList<(decimal, DateTime)> _lWld = new LinkedList<(decimal, DateTime)>();
        private const int N = 10;
        public static object _objLock = 1;
        private void Add_Eth(decimal item)
        {
            if (_lEth.Count >= N)
                _lEth.RemoveLast();
            _lEth.AddFirst((item, DateTime.Now));
        }
        private void Add_Btc(decimal item)
        {
            if (_lBtc.Count >= N)
                _lBtc.RemoveLast();
            _lBtc.AddFirst((item, DateTime.Now));
        }
        private void Add_Xbt(decimal item)
        {
            if (_lXbt.Count >= N)
                _lXbt.RemoveLast();
            _lXbt.AddFirst((item, DateTime.Now));
        }
        private void Add_Wld(decimal item)
        {
            if (_lWld.Count >= N)
                _lWld.RemoveLast();
            _lWld.AddFirst((item, DateTime.Now));
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

        private static Dictionary<string, List<QuoteEx>> _dicOB = new Dictionary<string, List<QuoteEx>>();
        public async Task DetectOrderBlock()
        {
            try
            {
                var dt = DateTime.Now;
                if (!((dt.Minute % 30 == 0
                    && dt.Second < 2) || !_dicOB.Any()))
                {
                    return;
                }
                var btc = "BTCUSDT";
                var eth = "ETHUSDT";
                var wld = "WLDUSDT";
                var aixbt = "AIXBTUSDT";

                var dicOB = new Dictionary<string, List<QuoteEx>>
                {
                    { btc, new List<QuoteEx>() },
                    { eth, new List<QuoteEx>() },
                    { wld, new List<QuoteEx>() },
                    { aixbt, new List<QuoteEx>() }
                };

               
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
                if(lOB_btc4h.Any())
                {
                    foreach (var item in lOB_btc4h)
                    {
                        item.Interval = (int)EIntervalMode.H4;
                    }
                    dicOB[btc].AddRange(lOB_btc4h);
                }    
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
                if (lOB_btc1h.Any())
                {
                    foreach (var item in lOB_btc1h)
                    {
                        item.Interval = (int)EIntervalMode.H1;
                    }
                    dicOB[btc].AddRange(lOB_btc1h);
                }
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
                if (lOB_btc15m.Any())
                {
                    foreach (var item in lOB_btc15m)
                    {
                        item.Interval = (int)EIntervalMode.M15;
                    }
                    dicOB[btc].AddRange(lOB_btc15m);
                }
                Thread.Sleep(100);

                
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
                if (lOB_eth4h.Any())
                {
                    foreach (var item in lOB_eth4h)
                    {
                        item.Interval = (int)EIntervalMode.H4;
                    }
                    dicOB[eth].AddRange(lOB_eth4h);
                }
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
                if (lOB_eth1h.Any())
                {   
                    foreach (var item in lOB_eth1h)
                    {
                        item.Interval = (int)EIntervalMode.H1;
                    }
                    dicOB[eth].AddRange(lOB_eth1h);
                }
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
                if (lOB_eth15m.Any())
                {   
                    foreach (var item in lOB_eth15m)
                    {
                        item.Interval = (int)EIntervalMode.M15;
                    }
                    dicOB[eth].AddRange(lOB_eth15m);
                }
                Thread.Sleep(100);

                
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
                if (lOB_wld4h.Any())
                {
                    foreach (var item in lOB_wld4h)
                    {
                        item.Interval = (int)EIntervalMode.H4;
                    }
                    dicOB[wld].AddRange(lOB_wld4h);
                }
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
                if (lOB_wld1h.Any())
                {   
                    foreach (var item in lOB_wld1h)
                    {
                        item.Interval = (int)EIntervalMode.H1;
                    }
                    dicOB[wld].AddRange(lOB_wld1h);
                }
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
                if (lOB_wld15m.Any())
                {   
                    foreach (var item in lOB_wld15m)
                    {
                        item.Interval = (int)EIntervalMode.M15;
                    }
                    dicOB[wld].AddRange(lOB_wld15m);
                }
                Thread.Sleep(100);

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
                if (lOB_aixbt4h.Any())
                {   
                    foreach (var item in lOB_aixbt4h)
                    {
                        item.Interval = (int)EIntervalMode.H4;
                    }
                    dicOB[aixbt].AddRange(lOB_aixbt4h);
                }
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
                if (lOB_aixbt1h.Any())
                {   
                    foreach (var item in lOB_aixbt1h)
                    {
                        item.Interval = (int)EIntervalMode.H1;
                    }
                    dicOB[aixbt].AddRange(lOB_aixbt1h);
                }
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
                if (lOB_aixbt15m.Any())
                {   
                    foreach (var item in lOB_aixbt15m)
                    {
                        item.Interval = (int)EIntervalMode.M15;
                    }
                    dicOB[aixbt].AddRange(lOB_aixbt15m);
                }
                Monitor.TryEnter(_objLock, TimeSpan.FromSeconds(100));
                _dicOB = dicOB;
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.CalculateCoin|EXCEPTION| {ex.Message}");
            }
        }

        public async Task<List<string>> CheckEntry()
        {
            var lRes = new List<string>();
            try
            {
                var dt = DateTime.Now;
                var btc = "BTCUSDT";
                Monitor.Enter(_objLock);
                var lastBTC = _lBtc.LastOrDefault();
                Monitor.Exit(_objLock);
                if(lastBTC.Item1 != null && (dt - lastBTC.Item2).TotalMinutes < 1)
                {
                    var focusShort = _dicOB[btc].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.TopPinbar || x.Mode == (int)EOrderBlockMode.TopInsideBar)
                                                            && lastBTC.Item1 >= x.Focus);
                    if (focusShort != null)
                    {
                        lRes.Add($"[OrderBlock - Short] BTC|{((EIntervalMode)focusShort.Interval).GetDisplayName()}|Entry: {focusShort.Entry}|SL: {focusShort.SL}");
                    }
                    var focusLong = _dicOB[btc].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.BotPinbar || x.Mode == (int)EOrderBlockMode.BotInsideBar)
                                                           && lastBTC.Item1 <= x.Focus);
                    if (focusLong != null)
                    {
                        lRes.Add($"[OrderBlock - Long] BTC|{((EIntervalMode)focusLong.Interval).GetDisplayName()}|Entry: {focusLong.Entry}|SL: {focusLong.SL}");
                    }
                }

                var eth = "ETHUSDT";
                Monitor.Enter(_objLock);
                var lastEth = _lEth.LastOrDefault();
                Monitor.Exit(_objLock);
                if (lastEth.Item1 != null && (dt - lastEth.Item2).TotalMinutes < 1)
                {
                    var focusShort = _dicOB[eth].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.TopPinbar || x.Mode == (int)EOrderBlockMode.TopInsideBar)
                                                            && lastEth.Item1 >= x.Focus);
                    if (focusShort != null)
                    {
                        lRes.Add($"[OrderBlock - Short] ETH|{((EIntervalMode)focusShort.Interval).GetDisplayName()}|Entry: {focusShort.Entry}|SL: {focusShort.SL}");
                    }
                    var focusLong = _dicOB[eth].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.BotPinbar || x.Mode == (int)EOrderBlockMode.BotInsideBar)
                                                           && lastEth.Item1 <= x.Focus);
                    if (focusLong != null)
                    {
                        lRes.Add($"[OrderBlock - Long] ETH|{((EIntervalMode)focusLong.Interval).GetDisplayName()}|Entry: {focusLong.Entry}|SL: {focusLong.SL}");
                    }
                }

                var wld = "WLDUSDT";
                Monitor.Enter(_objLock);
                var lastWLD = _lWld.LastOrDefault();
                Monitor.Exit(_objLock);
                if (lastWLD.Item1 != null && (dt - lastWLD.Item2).TotalMinutes < 1)
                {
                    var focusShort = _dicOB[wld].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.TopPinbar || x.Mode == (int)EOrderBlockMode.TopInsideBar)
                                                            && lastWLD.Item1 >= x.Focus);
                    if (focusShort != null)
                    {
                        lRes.Add($"[OrderBlock - Short] WLD|{((EIntervalMode)focusShort.Interval).GetDisplayName()}|Entry: {focusShort.Entry}|SL: {focusShort.SL}");
                    }
                    var focusLong = _dicOB[wld].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.BotPinbar || x.Mode == (int)EOrderBlockMode.BotInsideBar)
                                                           && lastWLD.Item1 <= x.Focus);
                    if (focusLong != null)
                    {
                        lRes.Add($"[OrderBlock - Long] WLD|{((EIntervalMode)focusLong.Interval).GetDisplayName()}|Entry: {focusLong.Entry}|SL: {focusLong.SL}");
                    }
                }

                var aixbt = "AIXBTUSDT";
                Monitor.Enter(_objLock);
                var lastXbt = _lXbt.LastOrDefault();
                Monitor.Exit(_objLock);
                if (lastXbt.Item1 != null && (dt - lastXbt.Item2).TotalMinutes < 1)
                {
                    var focusShort = _dicOB[aixbt].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.TopPinbar || x.Mode == (int)EOrderBlockMode.TopInsideBar)
                                                            && lastXbt.Item1 >= x.Focus);
                    if (focusShort != null)
                    {
                        lRes.Add($"[OrderBlock - Short] AIXBT|{((EIntervalMode)focusShort.Interval).GetDisplayName()}|Entry: {focusShort.Entry}|SL: {focusShort.SL}");
                    }
                    var focusLong = _dicOB[aixbt].FirstOrDefault(x => (x.Mode == (int)EOrderBlockMode.BotPinbar || x.Mode == (int)EOrderBlockMode.BotInsideBar)
                                                           && lastXbt.Item1 <= x.Focus);
                    if (focusLong != null)
                    {
                        lRes.Add($"[OrderBlock - Long] AIXBT|{((EIntervalMode)focusLong.Interval).GetDisplayName()}|Entry: {focusLong.Entry}|SL: {focusLong.SL}");
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"AnalyzeService.CheckEntry|EXCEPTION| {ex.Message}");
            }
            return lRes;
        }
    }
}