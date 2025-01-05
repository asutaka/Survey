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
            await _teleService.CalculateCoin();
            return;
            await _teleService.SubcribeCoin();
            while (!stoppingToken.IsCancellationRequested)
            {
                await _teleService.BotSyncUpdate();
                var dt = DateTime.Now;
                if(dt.Hour % 30 == 0
                    && dt.Minute == 0
                    && dt.Second < 2)
                {
                    await _teleService.CalculateCoin();
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
