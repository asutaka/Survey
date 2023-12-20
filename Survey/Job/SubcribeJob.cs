using Binance.Net.Clients;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using Quartz;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Survey.Job
{
    [DisallowConcurrentExecution]
    public class SubcribeJob : IJob
    {
        public static List<IBinanceMiniTick> _binanceTicks = new List<IBinanceMiniTick>();
        private static BinanceSocketClient _socket;
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"access Job: {SocketInstance().IncomingKbps}; Time: {DateTime.Now}");
            if (SocketInstance().IncomingKbps == 0)
            {
                NLogLogger.LogInfo($"retry Job: {DateTime.Now}");
                SocketInstance().UnsubscribeAllAsync();
                Thread.Sleep(2000);
                var binanceTick = _binanceTicks;
                var isLock = false;
                var subscribeResult = SocketInstance().SpotStreams.SubscribeToAllMiniTickerUpdatesAsync(data => {
                    if (!isLock)
                    {
                        isLock = true;
                        var lData = data.Data.Where(x => x.Symbol.EndsWith("USDT") 
                                                    && !x.Symbol.EndsWith("UPUSDT") 
                                                    && !x.Symbol.EndsWith("DOWNUSDT")).ToList();
                        var lExists = binanceTick.Where(x => lData.Any(y => y.Symbol.Equals(x.Symbol, StringComparison.InvariantCultureIgnoreCase)));
                        if (lExists != null && lExists.Any())
                        {
                            binanceTick = binanceTick.Except(lExists).ToList();
                        }
                        binanceTick.AddRange(lData);
                        _binanceTicks = binanceTick;
                        isLock = false;
                    }
                }).GetAwaiter().GetResult();

                subscribeResult.Data.ConnectionLost += () => NLogLogger.LogInfo("Connection lost");
                subscribeResult.Data.ConnectionRestored += (t) => NLogLogger.LogInfo("Connection restored");
            }
        }

        public static BinanceSocketClient SocketInstance()
        {
            _socket = _socket ?? new BinanceSocketClient(new BinanceSocketClientOptions() { });
            return _socket;
        }
    }
}
