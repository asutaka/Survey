using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using StockLib.DAL.Entity;
using StockLib.Model;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<Stream> Chart_NganHang_DoanhThu_LoiNhuan(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                if(!lMaCK.Contains("OCB"))
                {
                    lMaCK.Add("OCB");
                }    

                var configMain = _configMainRepo.GetAll().First();
                var d = int.Parse($"{configMain.year}{configMain.quarter}");
                var lFinancial = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, d));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, int.Parse($"{configMain.year - 1}{configMain.quarter}")));
                return await Chart_DoanhThuBase(lMaCK, configMain, d, lFinancial.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), null, isTangTruongLoiNhuan: true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_NganHang_TangTruongTinDung_RoomTinDung(IEnumerable<string> lInput)
        {
            try
            {
                var lNganHang = lInput.Take(15).ToList();
                if (!lNganHang.Contains("OCB"))
                {
                    lNganHang.Add("OCB");
                }

                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
                if (!lFinancial.Any())
                    return null;

                var lTangTruongTinDung = new List<double>();
                var lRoomTinDung = new List<double>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lTangTruongTinDung.Add(0);
                        lRoomTinDung.Add(0);
                        continue;
                    }

                    //tang truong tin dung, room tin dung
                    lTangTruongTinDung.Add(cur.credit_r ?? 0);
                    lRoomTinDung.Add(cur.room ?? 0);
                }

                var basicColumn = new HighchartTangTruongTinDung($"Tăng trưởng tín dụng Quý {configMain.quarter}/{configMain.year} (YoY)", lNganHang.ToList(), new List<HighChartSeries_TangTruongTinDung>
                {
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Room tín dụng",
                        type = "column",
                        data = lRoomTinDung,
                        color = "rgba(158, 159, 163, 0.5)",
                        pointPlacement = -0.2,
                        dataLabels = new HighChartDataLabel()
                    },
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Tăng trưởng tín dụng",
                        type = "column",
                        data = lTangTruongTinDung,
                        color = "#012060",
                        dataLabels = new HighChartDataLabel()
                    }
                });
                var strTitleYAxis = "(Đơn vị: %)";
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_TangTruongTinDung_RoomTinDung|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_NganHang_NoXau(IEnumerable<string> lInput)
        {
            try
            {
                var lNganHang = lInput.Take(15).ToList();
                if (!lNganHang.Contains("OCB"))
                {
                    lNganHang.Add("OCB");
                }

                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
                if (!lFinancial.Any())
                    return null;

                var yearPrev = configMain.year;
                var quarterPrev = configMain.quarter;
                if (configMain.quarter > 1)
                {
                    quarterPrev--;
                }
                else
                {
                    quarterPrev = 4;
                    yearPrev--;
                }
                var lFinancialPrev = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lBaoPhu = new List<double>();
                var lTiLeNo = new List<double>();
                var lTangTruongNoNhom2 = new List<double>();
                var lTangTruongTrichLap = new List<double>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lBaoPhu.Add(0);
                        lTiLeNo.Add(0);
                        lTangTruongNoNhom2.Add(0);
                        lTangTruongTrichLap.Add(0);
                        continue;
                    }
                    double baophu = 0, tileNoxau = 0, tangTruongTrichLap = 0, tangTruongNoNhom2 = 0;
                    var noxau = cur.debt3 + cur.debt4 + cur.debt5;
                    if (noxau > 0)
                    {
                        baophu = Math.Round(100 * (cur.risk ?? 0) / noxau, 1);
                    }
                    if (cur.debt > 0)
                    {
                        tileNoxau = Math.Round((float)(100 * noxau) / cur.debt, 1);
                    }

                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev != null)
                    {
                        if (prev.risk > 0)
                        {
                            tangTruongTrichLap = Math.Round(100 * (-1 + (cur.risk ?? 0) / (prev.risk ?? 1)), 1);
                        }
                        if (prev.debt2 > 0)
                        {
                            tangTruongNoNhom2 = Math.Round(100 * (-1 + (float)cur.debt2 / prev.debt2), 1);
                        }
                    }

                    //tang truong tin dung, room tin dung
                    lBaoPhu.Add(baophu);
                    lTiLeNo.Add(tileNoxau);
                    lTangTruongNoNhom2.Add(tangTruongNoNhom2);
                    lTangTruongTrichLap.Add(tangTruongTrichLap);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                     new() {
                        data = lBaoPhu,
                        name = "Bao phủ nợ xấu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    new()
                    {
                        data = lTiLeNo,
                        name = "Tỉ lệ nợ xấu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    },
                    new()
                    {
                        data = lTangTruongNoNhom2,
                        name = "Tăng trưởng nợ nhóm 2",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1
                    },
                    new()
                    {
                        data = lTangTruongTrichLap,
                        name = "Tăng trưởng trích lập",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                };
                return await Chart_BasicBase($"Nợ xấu Quý {configMain.quarter}/{configMain.year} (QoQ)", lNganHang, lSeries, "(Bao phủ nợ xấu: %)", "(Tăng trưởng: %)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_NoXau|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_NganHang_NimCasaChiPhiVon(IEnumerable<string> lInput)
        {
            try
            {
                var lNganHang = lInput.Take(15).ToList();
                if (!lNganHang.Contains("OCB"))
                {
                    lNganHang.Add("OCB");
                }
                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
                if (!lFinancial.Any())
                    return null;
                var lNim = new List<double>();
                var lCasa = new List<double>();
                var lCir = new List<double>();
                var lChiPhiVon = new List<double>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lNim.Add(0);
                        lCasa.Add(0);
                        lCir.Add(0);
                        lChiPhiVon.Add(0);
                        continue;
                    }

                    //tang truong tin dung, room tin dung
                    lNim.Add(cur.nim_r ?? 0);
                    lCasa.Add(cur.casa_r ?? 0);
                    lCir.Add(cur.cir_r ?? 0);
                    lChiPhiVon.Add(cur.cost_r ?? 0);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lNim,
                        name = "NIM",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lCasa,
                        name = "CASA",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lCir,
                        name = "CIR",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lChiPhiVon,
                        name = "Tăng trưởng chi phí vốn",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                };
                return await Chart_BasicBase($"NIM, CASA, CIR, Chi phí vốn Quý {configMain.quarter}/{configMain.year} (QoQ)", lNganHang, lSeries, "(NIM: %)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_NimCasaChiPhiVon|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
