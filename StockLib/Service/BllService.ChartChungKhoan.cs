using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using StockLib.DAL.Entity;
using StockLib.Model;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<Stream> Chart_CK_DoanhThu_LoiNhuan(IEnumerable<string> lInput)
        {
            try
            {
                var lChungKhoan = lInput.Take(15).ToList();
                var configMain = _configMainRepo.GetAll().First();
                var d = int.Parse($"{configMain.year}{configMain.quarter}");

                var lFinancial = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, d));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, int.Parse($"{configMain.year - 1}{configMain.quarter}")));
                return await Chart_DoanhThu_LoiNhuanBase(lChungKhoan, configMain, d, lFinancial.Select(x => (x.s, x.rv, x.pf)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        //public async Task<Stream> Chart_CK_TangTruongTinDung(IEnumerable<string> lInput)
        //{
        //    try
        //    {
        //        var lChungKhoan = lInput.Take(15).ToList();

        //        var configMain = _configMainRepo.GetAll().First();
        //        var lFinancial = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
        //        if (!lFinancial.Any())
        //            return null;

        //        var lResult = new List<HighChart_TinDung>();
        //        foreach (var item in lChungKhoan)
        //        {
        //            var cur = lFinancial.FirstOrDefault(x => x.s == item);
        //            if (cur is null)
        //            {
        //                lResult.Add(new HighChart_TinDung
        //                {
        //                    d = int.Parse($"{configMain.year}{configMain.quarter}"),
        //                    s = item
        //                });
        //                continue;
        //            }

        //            //tang truong tin dung, room tin dung
        //            lResult.Add(new HighChart_TinDung
        //            {
        //                s = cur.s,
        //                d = cur.d,
        //                TangTruongTinDung = cur.debt ?? 0,
        //                RoomTinDung = cur.room ?? 0
        //            });
        //        }

        //        var basicColumn = new HighchartTangTruongTinDung($"Tăng trưởng tín dụng Quý {configMain.quarter}/{configMain.year} (YoY)", lNganHang.ToList(), new List<HighChartSeries_TangTruongTinDung>
        //        {
        //            new HighChartSeries_TangTruongTinDung
        //            {
        //                name="Room tín dụng",
        //                type = "column",
        //                data = lResult.Select(x => x.RoomTinDung).ToList(),
        //                color = "rgba(158, 159, 163, 0.5)",
        //                pointPlacement = -0.2,
        //                dataLabels = new HighChartDataLabel()
        //            },
        //            new HighChartSeries_TangTruongTinDung
        //            {
        //                name="Tăng trưởng tín dụng",
        //                type = "column",
        //                data = lResult.Select(x => x.TangTruongTinDung).ToList(),
        //                color = "#012060",
        //                dataLabels = new HighChartDataLabel()
        //            }
        //        });
        //        var strTitleYAxis = "(Đơn vị: %)";
        //        basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
        //                                                         new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

        //        var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
        //        var body = JsonConvert.SerializeObject(chart);
        //        return await _apiService.GetChartImage(body);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //    }
        //    return null;
        //}
    }
}
