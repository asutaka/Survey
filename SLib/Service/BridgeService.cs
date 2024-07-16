using Microsoft.Extensions.DependencyInjection;
using SLib.DAL;
using SLib.Util;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SLib.Service
{
    public static class BridgeDI
    {
        public static void AddSLib(this IServiceCollection services)
        {
            services.DALDependencies();
            services.ServiceDependencies();
        }
    }

    public interface IBridgeService
    {
        Task TrenMa20(DateTime dt);
        Task NhomNganh(DateTime dt);
        Task GDNN(DateTime dt);
        Task TuDoanh(DateTime dt);
        Task ChiBaoKyThuat(DateTime dt);
    }

    public class BridgeService : IBridgeService
    {
        //private const long _idChannel = -1002247826353;
        //private const long _idUser = 1066022551;
        //private const long _idGroup = -4237476810;

        private const long _idMain = 1066022551;
        private readonly ITelegramLibService _telegramService;
        private readonly IBllService _bllService;
        public BridgeService(ITelegramLibService telegramService, IBllService bllService)
        {
            _telegramService = telegramService;
            _bllService = bllService;
        }

        public async Task GDNN(DateTime dt)
        {
            if ((int)dt.DayOfWeek < 1 || (int)dt.DayOfWeek > 5)
                return;

            if (dt.Hour != 15)
                return;

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
        }

        public async Task NhomNganh(DateTime dt)
        {
            if ((int)dt.DayOfWeek < 1 || (int)dt.DayOfWeek > 5)
                return;

            if (dt.Hour != 15)
                return;

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

        public async Task TrenMa20(DateTime dt)
        {
            if ((int)dt.DayOfWeek < 1 || (int)dt.DayOfWeek > 5)
                return;

            if (dt.Minute < 15 || dt.Minute >= 30 || dt.Hour < 9 || dt.Hour >= 15)
                return;
            try
            {
                var chibao = await _bllService.LayMaTheoChiBao();
                if (chibao.Item1 > 0)
                {
                    await _telegramService.BotInstance().SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task TuDoanh(DateTime dt)
        {
            if ((int)dt.DayOfWeek < 1 || (int)dt.DayOfWeek > 5)
                return;

            if (dt.Hour < 16 || dt.Hour > 18)
                return;

            //Tự doanh HNX
            try
            {
                var hnx = await _bllService.SyncTuDoanhHNX();
                if (hnx.Item1 > 0)
                {
                    await _telegramService.BotInstance().SendTextMessageAsync(_idMain, hnx.Item2);
                }
            }
            catch (Exception ex)
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

        public async Task ChiBaoKyThuat(DateTime dt)
        {
            if ((int)dt.DayOfWeek < 1 || (int)dt.DayOfWeek > 5)
                return;

            if (dt.Hour != 15)
                return;

            try
            {
                var cbkt = await _bllService.ChiBaoKyThuat(); ;
                if (cbkt.Item1 > 0)
                {
                    foreach (var item in cbkt.Item2)
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
