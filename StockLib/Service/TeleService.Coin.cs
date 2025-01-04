using Microsoft.Extensions.Logging;
using StockLib.Service.Settings;
using StockLib.Utils;
using System.Text;
using Telegram.Bot;

namespace StockLib.Service
{
    public partial class TeleService
    {
        private TelegramBotClient BotCoinInstance()
        {
            try
            {
                if (_botCoin == null)
                    _botCoin = new TelegramBotClient(ServiceSetting._botTokenCoin);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TeleService.BotCoinInstance|EXCEPTION| {ex.Message}");
            }

            return _botCoin;
        }

        private async Task AnalyzeCoin(long userId, string input)
        {
            return;
            var output = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }
            try
            {
                //var socketClient = new BinanceSocketClient();
                //var tickerSubscriptionResult = socketClient.SpotApi.ExchangeData.SubscribeToTickerUpdatesAsync("ETHUSDT", (update) =>
                //{
                //    var lastPrice = update.Data.LastPrice;
                //});
                //if ("off".Equals(input.Trim(), StringComparison.OrdinalIgnoreCase))
                //{
                //    var res = StaticVal.BybitSocketInstance().V5SpotApi.SubscribeToTickerUpdatesAsync("ETHUSDT", (update) =>
                //    {
                //        var lastPrice = update.Data.LastPrice;
                //        Console.WriteLine(lastPrice);
                //    });
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            input = input.RemoveSpace().ToUpper();
            if ((input.StartsWith("[") && input.EndsWith("]"))
                || (input.StartsWith("*") && input.EndsWith("*"))
                || (input.StartsWith("@") && input.EndsWith("@")))//Nhóm ngành
            {
                input = input.Replace("[", "").Replace("]", "").Replace("*", "").Replace("@", "");
                if (StaticVal._lBanLeKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Bán Lẻ
                {
                    await PrintImage(EStockType.BanLe.ToString(), userId, true);
                    await NganhBanLe(userId);
                    return;
                }
                if (StaticVal._lBatDongSanKey.Any(x => x.RemoveSpace().ToUpper().Equals(input)))//Ngành Bất động sản
                {
                    await PrintImage(EStockType.BDS.ToString(), userId, true);
                    await NganhBatDongSan(userId);
                    return;
                }
            }
        }

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
                _logger.LogError($"TeleService.SubcribeCoin|EXCEPTION| {ex.Message}");
            }
        }
    }
}
