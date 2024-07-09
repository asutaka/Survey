using Microsoft.Extensions.Hosting;
using Quartz;
using StockLibrary.Service;
using StockLibrary.Util;
using Telegram.Bot;

namespace StockBridgeAPI.Jobs
{
    [DisallowConcurrentExecution]
    public class BackgroundJob : IJob
    {
        private const long _idMain = -1002247826353;
        private readonly ITelegramLibService _telegramService;
        private readonly IBllService _bllService;
        public BackgroundJob(ITelegramLibService telegramService, IBllService bllService)
        {
            _telegramService = telegramService;
            _bllService = bllService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dt = DateTime.Now;
            if ((int)dt.DayOfWeek >= 1 && (int)dt.DayOfWeek <= 5)
            {
                if (dt.Hour >= 16 && dt.Hour <= 23)
                {
                    //Tự doanh HNX
                    try
                    {
                        var hnx = await _bllService.SyncTuDoanhHNX();
                        if (hnx.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, hnx.Item2);
                        }
                    }
                    catch(Exception ex)
                    {
                        NLogLogger.PublishException(ex, ex.Message);
                    }
                    //Tự doanh Upcom
                    try
                    {
                        var up = await _bllService.SyncTuDoanhUp();
                        if (up.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, up.Item2);
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogLogger.PublishException(ex, ex.Message);
                    }
                    //Tự doanh HSX
                    try
                    {
                        var hose = await _bllService.SyncTuDoanhHSX();
                        if (hose.Item1 > 0)
                        {
                            foreach (var item in hose.Item2)
                            {
                                await _telegramService.BotInstance().SendTextMessageAsync(_idMain, item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogLogger.PublishException(ex, ex.Message);
                    }
                    //GDNN
                    try
                    {
                        var nn = await _bllService.SyncGDNuocNgoai();
                        if(nn.Item1 > 0)
                        {
                            foreach (var item in nn.Item2)
                            {
                                await _telegramService.BotInstance().SendTextMessageAsync(_idMain, item);
                            }
                        }    
                    }
                    catch (Exception ex)
                    {
                        NLogLogger.PublishException(ex, ex.Message);
                    }
                }
            }
        }
    }
}
