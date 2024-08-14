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
                var lNganHang = lInput.Take(15).ToList();
                if(!lNganHang.Contains("OCB"))
                {
                    lNganHang.Add("OCB");
                }    

                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _nhRepo.GetByFilter(Builders<Financial_NH>.Filter.Eq(x => x.d, int.Parse($"{configMain.year - 1}{configMain.quarter}")));

                var lResult = new List<HighChart_LoiNhuanModel>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_LoiNhuanModel
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);

                    //tang truong doanh thu, tang truong loi nhuan
                    var model = new HighChart_LoiNhuanModel
                    {
                        s = cur.s,
                        d = cur.d, 
                        DoanhThu = (double)cur.rv,
                        LoiNhuan = (double)cur.pf
                    };
                    if (prev is null)
                    {
                        model.TangTruongDoanhThu = 0;
                        model.TangTruongLoiNhuan = 0;
                    }
                    else
                    {
                        var rateRevenue = (cur.rv / (prev.rv == 0 ? 0.1 : prev.rv));
                        var rateProfit = (cur.pf / (prev.pf == 0 ? 0.1 : prev.pf));

                        model.TangTruongDoanhThu = (double)Math.Round((-1 + rateRevenue) * 100, 1);
                        model.TangTruongLoiNhuan = (double)Math.Round((-1 + rateProfit) * 100, 1); ;
                    }
                    //Ty Suat Loi Nhuan
                    model.TySuatLoiNhuan = model.DoanhThu == 0 ? int.MaxValue : Math.Round(model.LoiNhuan * 100 / model.DoanhThu, 1);

                    lResult.Add(model);
                }
                var lDoanhThu = lResult.Select(x => x.DoanhThu).ToList();
                var lLoiNhuan = lResult.Select(x => x.LoiNhuan).ToList();
                var lTySuatLN = lResult.Select(x => x.TySuatLoiNhuan).ToList();
                var lTangTruongDoanhThu = lResult.Select(x => x.TangTruongDoanhThu).ToList();
                var lTangTruongLoiNhuan = lResult.Select(x => x.TangTruongLoiNhuan).ToList();

                var basicColumn = new HighchartBasicColumn($"Doanh Thu, Lợi Nhuận Quý {configMain.quarter}/{configMain.year} (QoQoY)", lNganHang.ToList(), new List<HighChartSeries_BasicColumn>
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
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTySuatLN,
                        name = "Tỷ suất LN",
                        type = "spline",
                        color = "#ffbf00",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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

                var lResult = new List<HighChart_TinDung>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_TinDung
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }
                    
                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_TinDung
                    {
                        s = cur.s,
                        d = cur.d,
                        TangTruongTinDung = cur.credit_r ?? 0,
                        RoomTinDung = cur.room ?? 0
                    });
                }

                var basicColumn = new HighchartTangTruongTinDung($"Tăng trưởng tín dụng Quý {configMain.quarter}/{configMain.year} (YoY)", lNganHang.ToList(), new List<HighChartSeries_TangTruongTinDung>
                {
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Room tín dụng",
                        type = "column",
                        data = lResult.Select(x => x.RoomTinDung).ToList(),
                        color = "rgba(158, 159, 163, 0.5)",
                        pointPlacement = -0.2,
                        dataLabels = new HighChartDataLabel()
                    },
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Tăng trưởng tín dụng",
                        type = "column",
                        data = lResult.Select(x => x.TangTruongTinDung).ToList(),
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
                _logger.LogError(ex.Message);
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

                var lResult = new List<HighChart_NoXau>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_NoXau
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
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
                    lResult.Add(new HighChart_NoXau
                    {
                        s = cur.s,
                        d = cur.d,
                        TongNoXau = cur.debt,
                        NoNhom1 = cur.debt1,
                        NoNhom2 = cur.debt2,
                        NoNhom3 = cur.debt3,
                        NoNhom4 = cur.debt4,
                        NoNhom5 = cur.debt5,
                        BaoPhuNoXau = baophu,
                        TileNoXau = tileNoxau,
                        TangTruongTrichLap = tangTruongTrichLap,
                        TangTruongNoNhom2 = tangTruongNoNhom2
                    });
                }

                var basicColumn = new HighchartBasicColumn($"Nợ xấu Quý {configMain.quarter}/{configMain.year} (QoQ)", lNganHang.ToList(), new List<HighChartSeries_BasicColumn>
                {
                     new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.BaoPhuNoXau).ToList(),
                        name = "Bao phủ nợ xấu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TileNoXau).ToList(),
                        name = "Tỉ lệ nợ xấu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TangTruongNoNhom2).ToList(),
                        name = "Tăng trưởng nợ nhóm 2",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TangTruongTrichLap).ToList(),
                        name = "Tăng trưởng trích lập",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                });
                var strTitleYAxis = "(Bao phủ nợ xấu: %)";
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = "(Tăng trưởng: %)" }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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

                var lResult = new List<HighChart_NimCasa>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_NimCasa
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }

                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_NimCasa
                    {
                        s = cur.s,
                        d = cur.d,
                        Nim = cur.nim_r ?? 0,
                        Casa = cur.casa_r ?? 0,
                        Cir = cur.cir_r ?? 0,
                        ChiPhiVon = cur.cost_r ?? 0
                    });
                }

                var basicColumn = new HighchartBasicColumn($"NIM, CASA, CIR, Chi phí vốn Quý {configMain.quarter}/{configMain.year} (QoQ)", lNganHang.ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.Nim).ToList(),
                        name = "NIM",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.Casa).ToList(),
                        name = "CASA",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.Cir).ToList(),
                        name = "CIR",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.ChiPhiVon).ToList(),
                        name = "Tăng trưởng chi phí vốn",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                });
                var strTitleYAxis = "(NIM: %)";
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = "(Tỉ lệ: %)" }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
