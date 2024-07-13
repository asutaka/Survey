using Quartz;
using SLib.Service;
using SLib.Util;
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
                if(dt.Hour >= 15 && dt.Hour < 16)
                {
                    //NN
                    try
                    {
                        var gdnn_day = await _bllService.SyncThongkeGDNN(E24hGDNNType.today);
                        if (gdnn_day.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, gdnn_day.Item2);
                        }

                        var gdnn_week = await _bllService.SyncThongkeGDNN(E24hGDNNType.week);
                        if (gdnn_week.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, gdnn_week.Item2);
                        }

                        var gdnn_month = await _bllService.SyncThongkeGDNN(E24hGDNNType.month);
                        if (gdnn_month.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, gdnn_month.Item2);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    //Nhóm ngành
                    try
                    {
                        var nhomNganh_day = await _bllService.SyncThongkeNhomNganh(E24hGDNNType.today);
                        if (nhomNganh_day.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, nhomNganh_day.Item2);
                        }

                        var nhomNganh_week = await _bllService.SyncThongkeNhomNganh(E24hGDNNType.week);
                        if (nhomNganh_week.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, nhomNganh_week.Item2);
                        }

                        var nhomNganh_month = await _bllService.SyncThongkeNhomNganh(E24hGDNNType.month);
                        if (nhomNganh_month.Item1 > 0)
                        {
                            await _telegramService.BotInstance().SendTextMessageAsync(_idMain, nhomNganh_month.Item2);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (dt.Hour >= 16 && dt.Hour <= 18)
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
                        Console.WriteLine(ex.Message);
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
                        Console.WriteLine(ex.Message);
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
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
