using StockLib.PublicService;

namespace TeleCoin
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITeleCoinService _teleService;
        private readonly long _channelCoin = -1002424403434;

        public Worker(ILogger<Worker> logger,
                    ITeleCoinService teleService)
        {
            _logger = logger;
            _teleService = teleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mesSend = string.Empty;
            await _teleService.SubcribeCoin();
            while (!stoppingToken.IsCancellationRequested)
            {
                await _teleService.BotSyncUpdate();
                await _teleService.DetectOrderBlock();
                var lMes = await _teleService.CheckEntry();
                if (lMes?.Any() ?? false)
                {
                    var mes = string.Join("\n", lMes.ToArray());
                    if (mes.Equals(mesSend))
                        continue;

                    mesSend = mes;
                    await _teleService.SendMessage(mes, _channelCoin);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
