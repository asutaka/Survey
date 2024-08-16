using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockLib.DAL.Entity;
using StockLib.Model;

namespace StockLib.Service
{
    public partial class BllService
    {
        //Item1: s; Item2: rv; Item3: pf
        private async Task<Stream> Chart_DoanhThu_LoiNhuanBase(List<string> lMaChungKhoan,
                                                                ConfigMain configMain,
                                                                int d,
                                                                IEnumerable<(string, double, double)> lCur,
                                                                IEnumerable<(string, double, double)> lPrev,
                                                                bool tysuatLN = false,
                                                                IEnumerable<(string, double)> lEx = null,
                                                                bool bienLNGop = false)
        {
            try
            {
                var lDoanhThu = new List<double>();
                var lLoiNhuan = new List<double>();
                var lTangTruongDoanhThu = new List<double>();
                var lTangTruongLoiNhuan = new List<double>();
                var lTySuatLoiNhuan = new List<double>();
                var lBienLoiNhuanGop = new List<double>();
                foreach (var item in lMaChungKhoan)
                {
                    var cur = lCur.FirstOrDefault(x => x.Item1 == item);
                    if (cur.Item1 is null)
                    {
                        lDoanhThu.Add(0);
                        lLoiNhuan.Add(0);
                        lTangTruongDoanhThu.Add(0);
                        lTangTruongLoiNhuan.Add(0);
                        lTySuatLoiNhuan.Add(0);
                        continue;
                    }
                    var prev = lPrev.FirstOrDefault(x => x.Item1 == item);

                    //tang truong doanh thu, tang truong loi nhuan
                    lDoanhThu.Add(cur.Item2);
                    lLoiNhuan.Add(cur.Item3);

                    if (prev.Item1 is null)
                    {
                        lTangTruongDoanhThu.Add(0);
                        lTangTruongLoiNhuan.Add(0);
                    }
                    else
                    {
                        var rateRevenue = (cur.Item2 / (prev.Item2 == 0 ? 0.1 : prev.Item2));
                        var rateProfit = (cur.Item3 / (prev.Item3 == 0 ? 0.1 : prev.Item3));
                        if (rateRevenue > 100)
                            rateRevenue = 100;
                        if(rateProfit > 100)
                            rateProfit = 100;

                        lTangTruongDoanhThu.Add(Math.Round((-1 + rateRevenue) * 100, 1));
                        lTangTruongLoiNhuan.Add(Math.Round((-1 + rateProfit) * 100, 1));
                    }
                    if(tysuatLN)
                    {
                        //Ty Suat Loi Nhuan
                        lTySuatLoiNhuan.Add(cur.Item2 == 0 ? 0 : Math.Round(cur.Item3 * 100 / cur.Item2, 1));
                    }
                    if (bienLNGop) 
                    {
                        if (lEx?.Any() ?? false)
                        {
                            lBienLoiNhuanGop.Add(Math.Round(lEx.FirstOrDefault(x => x.Item1 == item).Item2 * 100 / cur.Item2, 1));
                        }
                    }
                }
                var basicColumn = new HighchartBasicColumn($"Doanh Thu, Lợi Nhuận Quý {configMain.quarter}/{configMain.year} (QoQoY)", lMaChungKhoan.ToList(), new List<HighChartSeries_BasicColumn>
                {
                     new HighChartSeries_BasicColumn
                    {
                        data = lDoanhThu,
                        name = "Doanh thu",
                        type = "column",
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLoiNhuan,
                        name = "Lợi nhuận",
                        type = "column",
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongDoanhThu,
                        name = "Tăng trưởng DT",
                        type = "spline",
                        color = "#012060",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongLoiNhuan,
                        name = "Tăng trưởng LN",
                        type = "spline",
                        color = "#C00000",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    }
                });
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = "(Đơn vị: tỷ)" }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                if(tysuatLN)
                {
                    //Ty Suat Loi Nhuan
                    basicColumn.series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lTySuatLoiNhuan,
                        name = "Tỷ suất LN",
                        type = "spline",
                        color = "#ffbf00",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    });
                }
                if(bienLNGop)
                {
                    //Biên lợi nhuận gộp
                    basicColumn.series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lTySuatLoiNhuan,
                        name = "Biên LN Gộp",
                        type = "spline",
                        color = "#ffbf00",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    });
                }

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_DoanhThu_LoiNhuanBase|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        private async Task<Stream> Chart_BasicBase(string title, List<string> lCat, List<HighChartSeries_BasicColumn> lSerie, string titleX = null, string titleY = null)
        {
            try
            {
                var basicColumn = new HighchartBasicColumn(title, lCat, lSerie);
                var strX = string.IsNullOrWhiteSpace(titleX) ? "(Đơn vị: tỷ)" : titleX;
                var strY = string.IsNullOrWhiteSpace(titleY) ? "(Tỉ lệ: %)" : titleY;

                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strX }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = strY }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BasicBase|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
