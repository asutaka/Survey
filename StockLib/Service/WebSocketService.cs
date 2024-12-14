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
        private readonly ITeleService _teleService;
        public WebSocketService(IAPIService apiService, ITeleService teleService) 
        {
            _apiService = apiService;
            _teleService = teleService;
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

                    if (item.tradeTurnover < 15000)
                        continue;
                    var message = $"{item.exchangeName}|{item.baseCoin}|{item.posSide}|{item.tradeTurnover.ToString("#,##0.##")}";
                    Console.WriteLine(message);
                    var dat = await _apiService.CoinAnk_GetLiquidValue(item.contractCode);
                    Thread.Sleep(100);
                    try
                    {
                        if (dat is null || dat.data is null || dat.data.liqHeatMap is null)
                            continue;

                        var lLiquidLast = dat.data.liqHeatMap.data.Where(x => x.ElementAt(0) == 287);
                        var lPrice = await _apiService.GetCoinData_Binance(item.contractCode, "1h", DateTimeOffset.Now.AddHours(-12).ToUnixTimeMilliseconds());
                        if (!(lPrice?.Any() ?? false))
                            continue;

                        var flag = -1;
                        var curPrice = lPrice.Last().Close;
                        var count = dat.data.liqHeatMap.priceArray.Count();
                        for (var i = 0; i < count; i++)
                        {
                            var element = dat.data.liqHeatMap.priceArray[i];
                            if (element > curPrice)
                            {
                                flag = i; break;
                            }
                        }

                        if (flag < 0)
                            continue;

                        var minPrice = lPrice.Min(x => x.Low);
                        var maxPrice = lPrice.Max(x => x.High);
                        if (item.posSide.Equals("short", StringComparison.OrdinalIgnoreCase))
                        {
                            decimal priceMaxCeil = 0;
                            var maxCeil = lLiquidLast.Where(x => x.ElementAt(1) > flag).MaxBy(x => x.ElementAt(2));
                            if (maxCeil.ElementAt(2) >= (decimal)0.9 * dat.data.liqHeatMap.maxLiqValue)
                            {
                                priceMaxCeil = dat.data.liqHeatMap.priceArray[(int)maxCeil.ElementAt(1)];
                            }
                            if (priceMaxCeil <= 0)
                                continue;

                            if ((2 * priceMaxCeil + minPrice) <= 3 * curPrice
                                && (8 * priceMaxCeil + minPrice) > 9 * curPrice
                                && curPrice < maxPrice * (decimal)0.98) 
                            {
                                //Buy khi giá gần đến điểm thanh lý trên(2/3)
                                var mess = $"|LONG|{item.baseCoin}|Entry: {curPrice} --> Giá tăng gần đến điểm thanh lý({priceMaxCeil})|Đáy({minPrice})|(Giá trị: {maxCeil.ElementAt(2)}/{dat.data.liqHeatMap.maxLiqValue})";
                                await _teleService.SendTextMessageAsync(1066022551, mess);
                                Console.WriteLine(mess);
                            }
                            else if (curPrice > priceMaxCeil && curPrice >= maxPrice)
                            {
                                //Sell khi giá vượt qua điểm thanh lý trên
                                var mess = $"|SHORT|{item.baseCoin}|Entry: {curPrice} --> Giá tăng vượt qua điểm thanh lý điểm thanh lý: {priceMaxCeil}|Đáy({minPrice})|(Giá trị: {maxCeil.ElementAt(2)}/{dat.data.liqHeatMap.maxLiqValue})";
                                await _teleService.SendTextMessageAsync(1066022551, mess);
                                Console.WriteLine(mess);
                            }
                        }
                        else
                        {
                            var maxFloor = lLiquidLast.Where(x => x.ElementAt(1) < flag).MaxBy(x => x.ElementAt(2));
                            decimal priceMaxFloor = 0;
                            if (maxFloor.ElementAt(2) >= (decimal)0.9 * dat.data.liqHeatMap.maxLiqValue)
                            {
                                priceMaxFloor = dat.data.liqHeatMap.priceArray[(int)maxFloor.ElementAt(1)];
                            }
                            if (priceMaxFloor <= 0)
                                continue;

                            if ((maxPrice + 2 * priceMaxFloor) >= 3 * curPrice
                                && (maxPrice + 8 * priceMaxFloor) < 9 * curPrice
                                && curPrice > minPrice * (decimal)1.02)
                            {
                                //Sell khi giá gần đến điểm thanh lý dưới(1/3)
                                var mess = $"|SHORT|{item.baseCoin}|Entry: {curPrice} --> Giá giảm gần đến điểm thanh lý: {priceMaxFloor}|Đỉnh({maxPrice})|(Giá trị: {maxFloor.ElementAt(2)}/{dat.data.liqHeatMap.maxLiqValue})";
                                await _teleService.SendTextMessageAsync(1066022551, mess);
                                Console.WriteLine(mess);
                            }
                            else if (curPrice < priceMaxFloor && curPrice < minPrice)
                            {
                                //Buy khi giá gần đến điểm thanh lý dưới(1/3)
                                var mess = $"|LONG|{item.baseCoin}|Entry: {curPrice} --> Giá giảm vượt qua điểm thanh lý: {priceMaxFloor}|Đỉnh({maxPrice})|(Giá trị: {maxFloor.ElementAt(2)}/{dat.data.liqHeatMap.maxLiqValue})";
                                await _teleService.SendTextMessageAsync(1066022551, mess);
                                Console.WriteLine(mess);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    Console.WriteLine(JsonConvert.SerializeObject(item));
                }
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
