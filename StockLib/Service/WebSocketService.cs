using Newtonsoft.Json;
using System.Net.WebSockets;
using Websocket.Client;

namespace StockLib.Service
{
    public interface IWebSocketService
    {
        Task LiquidWebSocket(string url);
    }
    public class WebSocketService : IWebSocketService
    {
        private readonly IAPIService _apiService;
        public WebSocketService(IAPIService apiService) 
        {
            _apiService = apiService;
        }
        public async Task LiquidWebSocket(string url)
        {
            try
            {
                var uri = new Uri("ws://ws.coinank.com/wsKline/wsKline");
                var factory = new Func<ClientWebSocket>(() =>
                {
                    var client = new ClientWebSocket { Options = { KeepAliveInterval = TimeSpan.FromSeconds(10) } };
                    client.Options.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
                    return client;
                });

                var exitEvent = new ManualResetEvent(false);

                var client = new WebsocketClient(uri, factory);
                client.ReconnectTimeout = TimeSpan.FromSeconds(5);
                client.ReconnectionHappened.Subscribe(info =>
                {
                    Console.WriteLine($"Reconnection happened, type: {info.Type}");
                    client.Send("ping");
                    client.Send("{\"op\":\"subscribe\",\"args\":\"liqOrder@All@All@1m\"}");
                });

                client.MessageReceived.Subscribe(msg => HandleMessage(msg).GetAwaiter().GetResult());
                client.Start();
                Task.Run(() => client.Send("{\"op\":\"subscribe\",\"args\":\"liqOrder@All@All@1m\"}"));
                exitEvent.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task HandleMessage(ResponseMessage msg)
        {
            try
            {
                if (msg.Text.Length <= 50)
                    return;
                var res = JsonConvert.DeserializeObject<clsResponseMessage>(msg.Text);
                if(res.data is null)
                    return;

                foreach (var item in res.data)
                {
                    if (!"Binance".Equals(item.exchangeName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!_lSymbol.Contains(item.contractCode))
                        continue;

                    if (item.tradeTurnover >= 10000)
                    {
                        var dat = await _apiService.CoinAnk_GetLiquidValue(item.contractCode);
                        Console.WriteLine(JsonConvert.SerializeObject(item));
                    }
                }

                //Console.WriteLine($"Message received: {msg}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<string> _lSymbol = new List<string>
        {
            "BTCUSDT",
            "ETHUSDT",
            "XRPUSDT",
            "BNBUSDT",
            "SOLUSDT",
            "DOGEUSDT",
            "LTCUSDT",
            "BCHUSDT",
            "DOTUSDT",
            "LINKUSDT",
            "ARBUSDT",
            "WLDUSDT",
            "MATICUSDT",
            "ADAUSDT",
            "OPUSDT",
            "UNFIUSDT",
            "MKRUSDT",
            "PERPUSDT",
            "APTUSDT",
            "EOSUSDT",
            "SUIUSDT",
            "FILUSDT",
            "ETCUSDT",
            "AVAXUSDT",
            "CYBERUSDT",
            "ATOMUSDT",
            "APEUSDT",
            "DYDXUSDT",
            "TRBUSDT",
            "RUNEUSDT",
            "AGLDUSDT",
            "SUSHIUSDT",
            "GRTUSDT",
            "ANTUSDT",
            "1INCHUSDT",
            "JOEUSDT",
            "LINAUSDT",
            "GTCUSDT",
            "XTZUSDT",
            "NEOUSDT",
            "BELUSDT",
            "MAGICUSDT",
            "EGLDUSDT",
            "GMXUSDT",
            "IDUSDT",
            "THETAUSDT",
            "VETAUSDT",
            "KNCUSDT",
            "STORGUSDT",
            "AGIXUSDT",
            "OCEANUSDT",
            "STGUSDT",
            "C98USDT",
            "ZILUSDT",
            "MDTUSDT",
            "KLAYUSDT",
            "MINAUSDT",
            "HIGHUSDT",
            "STMXUSDT"
        };
    }

    public class clsResponseMessage
    {
        public List<clsResponseMessageDetail> data { get; set; }
    }
    public class clsResponseMessageDetail
    {
        public string exchangeName { get; set; }
        public string baseCoin { get; set; }
        public string side { get; set; }
        public string contractType { get; set; }
        public string contractCode { get; set; }
        public string posSide { get; set; }
        public decimal amount { get; set; }
        public decimal price { get; set; }
        public decimal tradeTurnover { get; set; }
        public long ts { get; set; }
    }
}
