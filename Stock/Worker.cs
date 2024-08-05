using StockLib.PublicService;

namespace Stock
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStockService _stockService;

        public Worker(ILogger<Worker> logger,
                        IStockService stockService)
        {
            _logger = logger;
            _stockService = stockService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _stockService.SyncDataMainBCTCFromWeb();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
