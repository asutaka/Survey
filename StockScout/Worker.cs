using SLib.Service;

namespace StockScout
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBridgeService _bridgeService;

        public Worker(ILogger<Worker> logger,
                        IBridgeService bridgeService)
        {
            _logger = logger;
            _bridgeService = bridgeService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var dt = DateTime.Now;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _bridgeService.TrenMa20(dt);
                await _bridgeService.NhomNganh(dt);
                await _bridgeService.GDNN(dt);
                await _bridgeService.TuDoanh(dt);

                await Task.Delay(1000 * 60 * 15, stoppingToken);
            }
        }
    }
}