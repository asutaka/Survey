using Quartz;
using SLib.Service;
using System.Threading.Tasks;

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
            await _telegramService.BotSyncUpdate();
        }
    }
}
