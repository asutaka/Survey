using Quartz;
using StockLibrary.Service;

namespace StockBridgeAPI.Jobs
{
    [DisallowConcurrentExecution]
    public class TelegramJob : IJob
    {
        private readonly ITelegramLibService _telegramService;
        public TelegramJob(ITelegramLibService telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //await _telegramService.BotSyncUpdate();
        }
    }
}
