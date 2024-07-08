using Quartz;
using StockLibrary.Service;
using Telegram.Bot;

namespace StockBridgeAPI.Jobs
{
    [DisallowConcurrentExecution]
    public class TelegramJob : IJob
    {
        private const long _idMain = -1002247826353;
        //private const long _idMain = 1066022551;
        private readonly ITelegramService _telegramService;
        private readonly IDataAPIService _apiService;
        public TelegramJob(ITelegramService telegramService, IDataAPIService apiService)
        {
            _telegramService = telegramService;
            _apiService = apiService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //var dt = DateTime.Now;
            //if((int)dt.DayOfWeek >= 1 && (int)dt.DayOfWeek <= 5) 
            //{
            //   if(dt.Hour >= 16 && dt.Hour <= 18)
            //}



            //if(dt.Minute %15 == 0
            ////if(dt.Minute == 0
            //    && dt.Second == 0
            //    && dt.Hour > 8
            //    && dt.Hour < 18)
            //{
            //    await _telegramService.BotInstance().SendTextMessageAsync(_idMain, "Working!!!");
            //}

            //await _telegramService.BotSyncUpdate();
        }
    }
}
