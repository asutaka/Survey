using Microsoft.Extensions.Logging;
using StockLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

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
        private const long _idMain = 1066022551;
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
        public async Task AnalyzeJob()
        {
            try
            {
                var dt = DateTime.Now;
                var isDayOfWork = dt.DayOfWeek >= DayOfWeek.Monday && dt.DayOfWeek <= DayOfWeek.Friday;
                var isTimePrint = dt.Minute >= 15 && dt.Minute < 30;
                var isRealTime = dt.Hour >= 9 && dt.Hour < 15;
                var isPreTrade = dt.Hour < 9;

                try
                {
                    //var chibao = await _analyzeService.TongCucThongKeThang(dt.AddMonths(-1));//for test
                    //if (chibao.Item1 > 0)
                    //{
                    //    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                    //}

                    var chibaoQuy = await _analyzeService.TongCucThongKeQuy(dt.AddMonths(-2));
                    if (chibaoQuy.Item1 > 0)
                    {
                        await _teleService.SendTextMessageAsync(_idMain, chibaoQuy.Item2);
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(TongCucThongKe)| {ex.Message}");
                }
                return;

                try
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        //var chibao = await _analyzeService.TongCucThongKeThang(dt.AddMonths(-i));//for test
                        //if (chibao.Item1 > 0)
                        //{
                        //    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                        //}

                        var chibaoQuy = await _analyzeService.TongCucThongKeQuy(dt.AddMonths(-i));
                        if (chibaoQuy.Item1 > 0)
                        {
                            await _teleService.SendTextMessageAsync(_idMain, chibaoQuy.Item2);
                            Thread.Sleep(1000);
                        }
                    }

                    for (int i = 11; i <= 19; i++)
                    {
                        //var chibao = await _analyzeService.TongCucThongKeThangTest(dt.AddMonths(-i));//for test
                        //if (chibao.Item1 > 0)
                        //{
                        //    await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                        //}

                        var chibaoQuy = await _analyzeService.TongCucThongKeQuyTest(dt.AddMonths(-i));
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
                return;


                if (isDayOfWork && isTimePrint && !isPreTrade)
                {
                    #region RealTime
                    if(isRealTime)
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
                            _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ChiBaoMA20)| {ex.Message}");
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
                            _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ChiBao52W)| {ex.Message}");
                        }

                        return;
                    }
                    #endregion
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
                        _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ThongkeNhomNganh)| {ex.Message}");
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
                        _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ThongkeForeign)| {ex.Message}");
                    }

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
                        _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ThongKeTuDoanhHNX)| {ex.Message}");
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
                        _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ThongKeTuDoanhUp)| {ex.Message}");
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
                        _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ThongKeTuDoanhHSX)| {ex.Message}");
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
                        _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(ChiBaoKyThuat)| {ex.Message}");
                    }
                }
                else
                {
                    if (dt.Day >= 28 && dt.Hour >= 15)
                    {
                        //Tổng cục thống kê
                        try
                        {
                            var chibao = await _analyzeService.TongCucThongKeThang(dt);
                            if (chibao.Item1 > 0)
                            {
                                await _teleService.SendTextMessageAsync(_idMain, chibao.Item2);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION(TongCucThongKe)| {ex.Message}");
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                _logger.LogError($"AnalyzeStockService.AnalyzeJob|EXCEPTION| {ex.Message}");
            }
        }
    }
}
