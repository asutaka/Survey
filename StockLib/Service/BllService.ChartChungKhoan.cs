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
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();

                var lFinancial = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, int.Parse($"{time.Item2 - 1}{time.Item3}")));
                return await Chart_DoanhThuBase(lMaCK, (int)time.Item1, lFinancial.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), null, isTangTruongLoiNhuan: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task<Stream> Chart_CK_TangTruongTinDung_RoomTinDung(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();

                var lFinancial = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var yearPrev = time.Item2;
                var quarterPrev = time.Item3;
                if (time.Item3 > 1)
                {
                    quarterPrev--;
                }
                else
                {
                    quarterPrev = 4;
                    yearPrev--;
                }
                var lFinancialPrev = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lMargin = new List<double>();
                var lVonChu = new List<double>();
                var lMarginTrenVonChu = new List<double>();
                var lTangTruongMargin = new List<double>();
                foreach (var item in lMaCK)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lMargin.Add(0);
                        lMarginTrenVonChu.Add(0);
                        continue;
                    }

                    //tang truong tin dung, room tin dung
                    lMargin.Add(cur.debt);
                    lVonChu.Add(cur.eq);
                    double marginTrenVonChu = 0;
                    if(cur.eq > 0)
                    {
                        marginTrenVonChu = Math.Round(cur.debt * 100 / cur.eq, 1);
                    }    
                    lMarginTrenVonChu.Add(marginTrenVonChu);
                    //
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    double tangTruongMargin = 0;
                    if(prev is not null && prev.idebt != 0)
                    {
                        tangTruongMargin = Math.Round(100*(-1 + cur.idebt / prev.idebt), 1);
                    }
                    lTangTruongMargin.Add(tangTruongMargin);
                }

                var basicColumn = new HighchartTangTruongTinDung($"Tăng trưởng Margin Quý {time.Item3}/{time.Item2} (QoQ)", lMaCK.ToList(), new List<HighChartSeries_TangTruongTinDung>
                {
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Vốn chủ sở hữu",
                        type = "column",
                        data = lVonChu,
                        color = "rgba(158, 159, 163, 0.5)",
                        pointPlacement = -0.2,
                        dataLabels = new HighChartDataLabel()
                    },
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Dư nợ Margin",
                        type = "column",
                        data = lMargin,
                        color = "#012060",
                        dataLabels = new HighChartDataLabel()
                    },
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Tăng trưởng lãi Margin",
                        type = "spline",
                        data = lTangTruongMargin,
                        color = "#C00000",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    },
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Margin trên vốn chủ",
                        type = "spline",
                        data = lMarginTrenVonChu,
                        color = "#ffbf00",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    }
                });
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = "(Đơn vị: tỷ)" }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = "(Đơn vị: %)" }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_CK_TangTruongTinDung_RoomTinDung|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_CK_MoiGioi(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();

                var lFinancial = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var lDoanhThuMoiGioi = new List<double>();
                var lLoiNhuanMoiGioi = new List<double>();
                var lBienLoiNhuanMoiGioi = new List<double>();
                foreach (var item in lMaCK)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lDoanhThuMoiGioi.Add(0);
                        lLoiNhuanMoiGioi.Add(0);
                        lBienLoiNhuanMoiGioi.Add(0);
                        continue;
                    }

                    double loiNhuanMoiGioi = cur.broker - cur.bcost;
                    double bienLoiNhuan = 0;
                    if (cur.broker != 0)
                    {
                        bienLoiNhuan = Math.Round(100 * loiNhuanMoiGioi / cur.broker, 1);
                    }

                    //tang truong tin dung, room tin dung
                    lDoanhThuMoiGioi.Add(cur.broker);
                    lLoiNhuanMoiGioi.Add(loiNhuanMoiGioi);
                    lBienLoiNhuanMoiGioi.Add(bienLoiNhuan);
                }

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lDoanhThuMoiGioi,
                        name = "Doanh thu môi giới",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLoiNhuanMoiGioi,
                        name = "Lợi nhuận môi giới",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lBienLoiNhuanMoiGioi,
                        name = "Biên lợi nhuận",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Thống kê môi giới {time.Item3}/{time.Item2}", lMaCK, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_CK_MoiGioi|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_CK_TuDoanh(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();

                var lFinancial = _ckRepo.GetByFilter(Builders<Financial_CK>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var lTaiSanTuDoanh = new List<double>();
                var lLoiNhuanTuDoanh = new List<double>();
                var lLoiNhuanTrenTaiSanTuDoanh = new List<double>();
                foreach (var item in lMaCK)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lTaiSanTuDoanh.Add(0);
                        lLoiNhuanTuDoanh.Add(0);
                        lLoiNhuanTrenTaiSanTuDoanh.Add(0);
                        continue;
                    }

                    double loiNhuanTrenTaiSan = 0;
                    if (cur.itrade != 0)
                    {
                        loiNhuanTrenTaiSan = Math.Round(100 * cur.trade / cur.itrade, 1);
                    }

                    //tang truong tin dung, room tin dung
                    lTaiSanTuDoanh.Add(cur.itrade);
                    lLoiNhuanTuDoanh.Add(cur.trade);
                    lLoiNhuanTrenTaiSanTuDoanh.Add(loiNhuanTrenTaiSan);
                }

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTaiSanTuDoanh,
                        name = "Tài sản tự doanh",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLoiNhuanTuDoanh,
                        name = "Lợi nhuận tự doanh",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLoiNhuanTrenTaiSanTuDoanh,
                        name = "Biên lợi nhuận trên tài sản",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Thống kê tự doanh {time.Item3}/{time.Item2}", lMaCK, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_CK_TuDoanh|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
