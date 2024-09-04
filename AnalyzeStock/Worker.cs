using StockLib.PublicService;

namespace AnalyzeStock
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAnalyzeStockService _analyzeService;

        public Worker(ILogger<Worker> logger,
                    IAnalyzeStockService analyzeService)
        {
            _logger = logger;
            _analyzeService = analyzeService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _analyzeService.AnalyzeJob();
                //await Task.Delay(1000, stoppingToken);
                await Task.Delay(1000 * 60 * 15, stoppingToken);
            }
        }
    }
}
