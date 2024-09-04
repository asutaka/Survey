﻿using Microsoft.Extensions.Logging;
using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IAnalyzeStockService
    {
        Task AnalyzeJob();
    }
    public class AnalyzeStockService : IAnalyzeStockService
    {
        private readonly ILogger<AnalyzeStockService> _logger;
        private readonly ITeleService _teleService;
        private readonly IAnalyzeService _analyzeService;
        private readonly ICalculateService _calculateService;

        //private const long _idChannel = -1002247826353;
        //private const long _idUser = 1066022551;
        //private const long _idGroup = -4237476810;
        private const long _idMain = -1002247826353;
        public AnalyzeStockService(ILogger<AnalyzeStockService> logger,
                                    ITeleService teleService,
                                    IAnalyzeService analyzeService,
                                    ICalculateService calculateService)
        {
            _logger = logger;
            _teleService = teleService;
            _analyzeService = analyzeService;
            _calculateService = calculateService;
        }

        private async Task Realtime()
        {
            //Chỉ báo cắt lên MA20
            try
            {
                var chibao = await _analyzeService.ChiBaoMA20();
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.Realtime|EXCEPTION(ChiBaoMA20)| {ex.Message}");
            }

            //Chỉ báo vượt đỉnh 52 Tuần(1 năm)
            try
            {
                var chibao = await _analyzeService.ChiBao52W();
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.Realtime|EXCEPTION(ChiBao52W)| {ex.Message}");
            }
        }
        private async Task ThongKe(DateTime dt)
        {
            //Thống kê nhóm ngành
            try
            {
                var chibao = await _analyzeService.ThongkeNhomNganh(dt);
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.ThongKe|EXCEPTION(ThongkeNhomNganh)| {ex.Message}");
            }

            //Thống kê Foreign
            try
            {
                var chibao = await _analyzeService.ThongkeForeign(dt);
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.ThongKe|EXCEPTION(ThongkeForeign)| {ex.Message}");
            }

            //Chỉ báo kỹ thuật
            try
            {
                var chibao = await _calculateService.ChiBaoKyThuat(dt);
                if (chibao.Item1 > 0)
                {
                    foreach (var item in chibao.Item2)
                    {
                        await _teleService.SendTextMessageAsync(_idMain, item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.ThongKe|EXCEPTION(ChiBaoKyThuat)| {ex.Message}");
            }
        }
        private async Task ThongKeTuDoanh(DateTime dt)
        {
            //Thống kê Tự doanh HNX
            try
            {
                var chibao = await _analyzeService.ThongKeTuDoanhHNX(dt);
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.ThongKeTuDoanh|EXCEPTION(ThongKeTuDoanhHNX)| {ex.Message}");
            }

            //Thống kê Tự doanh Upcom
            try
            {
                var chibao = await _analyzeService.ThongKeTuDoanhUp(dt);
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.ThongKeTuDoanh|EXCEPTION(ThongKeTuDoanhUp)| {ex.Message}");
            }

            //Thống kê Tự doanh HSX
            try
            {
                var chibao = await _analyzeService.ThongKeTuDoanhHSX(dt);
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.ThongKeTuDoanh|EXCEPTION(ThongKeTuDoanhHSX)| {ex.Message}");
            }
        }
        private async Task TongCucHaiQuan(DateTime dt)
        {
            try
            {
                var haiquanXK = await _analyzeService.TongCucHaiQuan(dt, Utils.EConfigDataType.TongCucHaiQuan_XK);
                if (haiquanXK.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, haiquanXK.Item2);
                    Thread.Sleep(1000);
                }

                var haiquanNK = await _analyzeService.TongCucHaiQuan(dt, Utils.EConfigDataType.TongCucHaiQuan_NK);
                if (haiquanNK.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, haiquanNK.Item2);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.ThongKeHaiQuan|EXCEPTION(ThongKeTuDoanhUp)| {ex.Message}");
            }
        }
        private async Task TongCucThongKe(DateTime dt)
        {
            try
            {
                var chibao = await _analyzeService.TongCucThongKeThang(dt);
                if (chibao.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                    Thread.Sleep(1000);
                }

                if (dt.Month % 3 <= 1)
                {
                    var chibaoQuy = await _analyzeService.TongCucThongKeQuy(dt);
                    if (chibaoQuy.Item1 > 0)
                    {
                        await _teleService.SendTextMessageAsync(_idMain, chibaoQuy.Item2);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(TongCucThongKe)| {ex.Message}");
            }
        }

        public async Task AnalyzeJob()
        {
            try
            {
                var dt = DateTime.Now;
                var isDayOfWork = dt.DayOfWeek >= DayOfWeek.Monday && dt.DayOfWeek <= DayOfWeek.Friday;//Từ thứ 2 đến thứ 6
                var isTimePrint = dt.Minute >= 15 && dt.Minute < 30;//từ phút thứ 15 đến phút thứ 30
                var isRealTime = dt.Hour >= 9 && dt.Hour < 15;//từ 9h đến 3h
                var isPreTrade = dt.Hour < 9;

                //try
                //{
                //    for (int i = 1; i <= 10; i++)
                //    {
                //        var dtTemp = dt.AddMonths(-i);
                //        var chibao = await _analyzeService.TongCucThongKeThang(dtTemp);//for test
                //        await _analyzeService.TongCucThongKeQuy(dtTemp);
                //        ////if (chibao.Item1 > 0)
                //        ////{
                //        ////    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                //        ////}
                //    }

                //    for (int i = 11; i <= 19; i++)
                //    {
                //        var dtTemp = dt.AddMonths(-i);
                //        var chibao = await _analyzeService.TongCucThongKeThangTest(dtTemp);//for test
                //        await _analyzeService.TongCucThongKeQuy(dtTemp);
                //        //if (chibao.Item1 > 0)
                //        //{
                //        //    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                //        //}
                //    }
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(TongCucThongKe)| {ex.Message}");
                //}
                //return;

                if (isDayOfWork && isTimePrint && !isPreTrade)
                {
                    if (isRealTime)
                    {
                        await Realtime();
                        return;
                    }
                    await ThongKe(dt);
                    await ThongKeTuDoanh(dt);
                }

                if ((dt.Day >= 28 || dt.Day <= 5))
                {
                    await TongCucThongKe(dt);
                }

                await TongCucHaiQuan(dt);
            }
            catch(Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION| {ex.Message}");
            }
        }
    }
}
