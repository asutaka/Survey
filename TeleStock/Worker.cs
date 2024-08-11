using StockLib.PublicService;

namespace TeleStock
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITeleStockService _teleStockService;

        public Worker(ILogger<Worker> logger,
                    ITeleStockService teleStockService)
        {
            _logger = logger;
            _teleStockService = teleStockService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _teleStockService.BotSyncUpdate();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
