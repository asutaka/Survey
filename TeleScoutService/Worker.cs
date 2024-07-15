using SLib.Service;

namespace TeleScoutService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITelegramLibService _telegramService;

        public Worker(ILogger<Worker> logger,
                      ITelegramLibService telegramService)
        {
            _logger = logger;
            _telegramService = telegramService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _telegramService.BotSyncUpdate();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}