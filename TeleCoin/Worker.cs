using StockLib.PublicService;

namespace TeleCoin
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITeleCoinService _teleService;

        public Worker(ILogger<Worker> logger,
                    ITeleCoinService teleService)
        {
            _logger = logger;
            _teleService = teleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _teleService.SubcribeCoin();
            while (!stoppingToken.IsCancellationRequested)
            {
                await _teleService.BotSyncUpdate();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
