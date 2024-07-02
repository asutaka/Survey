using Quartz;
using StockLibrary.Service;
using Telegram.Bot;

namespace StockBridgeAPI.Jobs
{
    [DisallowConcurrentExecution]
    public class BackgroundJob : IJob
    {
        private const long _idMain = 1066022551;
        private readonly ITelegramService _telegramService;
        private readonly IBllService _bllService;
        public BackgroundJob(ITelegramService telegramService, IBllService bllService)
        {
            _telegramService = telegramService;
            _bllService = bllService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dt = DateTime.Now;
            if ((int)dt.DayOfWeek >= 1 && (int)dt.DayOfWeek <= 5)
            {
                if (dt.Hour >= 16 && dt.Hour <= 18)
                {
                    var hnx = await _bllService.SyncTuDoanhHNX();
                    if (hnx.Item1 > 0)
                    {
                        await _telegramService.BotInstance().SendTextMessageAsync(_idMain, hnx.Item2);
                    }

                    var up = await _bllService.SyncTuDoanhUp();
                    if (up.Item1 > 0)
                    {
                        await _telegramService.BotInstance().SendTextMessageAsync(_idMain, up.Item2);
                    }
                }
            }



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
