using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<Stream> Chart_BDS_DoanhThu_LoiNhuan(IEnumerable<string> lInput)
        {
            try
            {
                var lBDS = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
                                        ).Take(15).ToList();
                lBDS.Remove("KSF");
                lBDS.Remove("VPI");
                lBDS.Add("DPG");
                lBDS.Add("NTL");

                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
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
                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lResult = new List<HighChart_LoiNhuanModel>();
                foreach (var item in lBDS)
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
                        var rateRevenue = (cur.rv / (prev.rv == 0 ? cur.rv : prev.rv));
                        var rateProfit = (cur.pf / (prev.pf == 0 ? cur.pf : prev.pf));

                        model.TangTruongDoanhThu = (double)Math.Round((-1 + rateRevenue) * 100, 1);
                        model.TangTruongLoiNhuan = (double)Math.Round((-1 + rateProfit) * 100, 1); ;
                    }
                    //Ty Suat Loi Nhuan
                    model.TySuatLoiNhuan = cur.ce == 0 ? 0 : Math.Round(100 * (-1 + model.DoanhThu / cur.ce), 1);

                    lResult.Add(model);
                }
                var lDoanhThu = lResult.Select(x => x.DoanhThu).ToList();
                var lLoiNhuan = lResult.Select(x => x.LoiNhuan).ToList();
                var lTySuatLN = lResult.Select(x => x.TySuatLoiNhuan).ToList();
                var lTangTruongDoanhThu = lResult.Select(x => x.TangTruongDoanhThu).ToList();
                var lTangTruongLoiNhuan = lResult.Select(x => x.TangTruongLoiNhuan).ToList();

                var basicColumn = new HighchartBasicColumn($"Doanh Thu, Lợi Nhuận Quý {configMain.quarter}/{configMain.year}", lBDS.ToList(), new List<HighChartSeries_BasicColumn>
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
                    //new HighChartSeries_BasicColumn
                    //{
                    //    data = lTySuatLN,
                    //    name = "Biên LN",
                    //    type = "spline",
                    //    color = "#ffbf00",
                    //    dataLabels = new HighChartDataLabel(),
                    //    yAxis = 1
                    //}
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

        public async Task<Stream> Chart_BDS_TonKho(IEnumerable<string> lInput)
        {
            try
            {
                var lBDS = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
                                       ).Take(15).ToList();
                lBDS.Remove("KSF");
                lBDS.Remove("VPI");
                lBDS.Add("DPG");
                lBDS.Add("NTL");

                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
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
                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lResult = new List<HighChart_TonKho>();
                foreach (var item in lBDS)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_TonKho
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.inv > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_TonKho
                    {
                        s = cur.s,
                        d = cur.d,
                        TonKho = cur.inv,
                        TangTruong = tangTruong
                    });
                }

                var basicColumn = new HighchartBasicColumn($"Tồn kho Quý {configMain.quarter}/{configMain.year}", lBDS.ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TonKho).ToList(),
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TangTruong).ToList(),
                        name = "Tăng trưởng tồn kho",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
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

        public async Task<Stream> Chart_BDS_NguoiMua(IEnumerable<string> lInput)
        {
            try
            {
                var lBDS = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
                                       ).Take(15).ToList();
                lBDS.Remove("KSF");
                lBDS.Remove("VPI");
                lBDS.Add("DPG");
                lBDS.Add("NTL");

                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
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
                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lResult = new List<HighChart_NguoiMua>();
                foreach (var item in lBDS)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_NguoiMua
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.bp > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.bp / prev.bp), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_NguoiMua
                    {
                        s = cur.s,
                        d = cur.d,
                        NguoiMua = cur.bp,
                        TangTruong = tangTruong
                    });
                }

                var basicColumn = new HighchartBasicColumn($"Người mua trả tiền trước Quý {configMain.quarter}/{configMain.year}", lBDS.ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.NguoiMua).ToList(),
                        name = "Người mua trả tiền trước",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TangTruong).ToList(),
                        name = "Tăng trưởng người mua",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
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

        public async Task<Stream> Chart_BDS_NoTrenVonChu(IEnumerable<string> lInput)
        {
            try
            {
                var lBDS = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
                                       ).Take(15).ToList();
                lBDS.Remove("KSF");
                lBDS.Remove("VPI");
                lBDS.Add("DPG");
                lBDS.Add("NTL");

                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
                if (!lFinancial.Any())
                    return null;

                var lResult = new List<HighChart_NoTrenVonChu>();
                foreach (var item in lBDS)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_NoTrenVonChu
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }

                    double noTrenVonChu = 0;
                    if (cur.eq != 0)
                    {
                        var sign = cur.eq >= cur.debt;

                        noTrenVonChu = Math.Abs(Math.Round(100 * (cur.debt / cur.eq), 1));
                        if (!sign)
                        {
                            noTrenVonChu = -noTrenVonChu;
                        }
                    }

                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_NoTrenVonChu
                    {
                        s = cur.s,
                        d = cur.d,
                        VonChu = cur.eq,
                        No = cur.debt,
                        NoTrenVonChu = noTrenVonChu
                    });
                }

                var basicColumn = new HighchartBasicColumn($"Nợ trên vốn chủ sở hữu Quý {configMain.quarter}/{configMain.year}", lBDS.ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.VonChu).ToList(),
                        name = "Vốn chủ sở hữu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.No).ToList(),
                        name = "Nợ",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.NoTrenVonChu).ToList(),
                        name = "Nợ trên vố chủ sở hữu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
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


        public async Task<Stream> Chart_VIN_DoanhThu_LoiNhuan()
        {
            try
            {
                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
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
                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lResult = new List<HighChart_LoiNhuanModel>();
                foreach (var item in StaticVal._lVin)
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
                        var rateRevenue = (cur.rv / (prev.rv == 0 ? cur.rv : prev.rv));
                        var rateProfit = (cur.pf / (prev.pf == 0 ? cur.pf : prev.pf));

                        model.TangTruongDoanhThu = (double)Math.Round((-1 + rateRevenue) * 100, 1);
                        model.TangTruongLoiNhuan = (double)Math.Round((-1 + rateProfit) * 100, 1); ;
                    }
                    //Ty Suat Loi Nhuan
                    model.TySuatLoiNhuan = cur.ce == 0 ? 0 : Math.Round(100 * (-1 + model.DoanhThu / cur.ce), 1);

                    lResult.Add(model);
                }
                var lDoanhThu = lResult.Select(x => x.DoanhThu).ToList();
                var lLoiNhuan = lResult.Select(x => x.LoiNhuan).ToList();
                var lTySuatLN = lResult.Select(x => x.TySuatLoiNhuan).ToList();
                var lTangTruongDoanhThu = lResult.Select(x => x.TangTruongDoanhThu).ToList();
                var lTangTruongLoiNhuan = lResult.Select(x => x.TangTruongLoiNhuan).ToList();

                var basicColumn = new HighchartBasicColumn($"Doanh Thu, Lợi Nhuận Quý {configMain.quarter}/{configMain.year}", StaticVal._lVin.ToList(), new List<HighChartSeries_BasicColumn>
                {
                     new HighChartSeries_BasicColumn
                    {
                        data = lDoanhThu,
                        name = "Doanh thu",
                        type = "column",
                        color = "#012060",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" }
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLoiNhuan,
                        name = "Lợi nhuận",
                        type = "column",
                        color = "#C00000",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" }
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
                    //new HighChartSeries_BasicColumn
                    //{
                    //    data = lTySuatLN,
                    //    name = "Biên LN",
                    //    type = "spline",
                    //    color = "#ffbf00",
                    //    dataLabels = new HighChartDataLabel(),
                    //    yAxis = 1
                    //}
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

        public async Task<Stream> Chart_VIN_TonKho()
        {
            try
            {
                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
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
                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lResult = new List<HighChart_TonKho>();
                foreach (var item in StaticVal._lVin)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_TonKho
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.inv > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_TonKho
                    {
                        s = cur.s,
                        d = cur.d,
                        TonKho = cur.inv,
                        TangTruong = tangTruong
                    });
                }

                var basicColumn = new HighchartBasicColumn($"Tồn kho Quý {configMain.quarter}/{configMain.year}", StaticVal._lVin.ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TonKho).ToList(),
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TangTruong).ToList(),
                        name = "Tăng trưởng tồn kho",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
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

        public async Task<Stream> Chart_VIN_NguoiMua()
        {
            try
            {
                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
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
                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lResult = new List<HighChart_NguoiMua>();
                foreach (var item in StaticVal._lVin)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_NguoiMua
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.bp > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.bp / prev.bp), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_NguoiMua
                    {
                        s = cur.s,
                        d = cur.d,
                        NguoiMua = cur.bp,
                        TangTruong = tangTruong
                    });
                }

                var basicColumn = new HighchartBasicColumn($"Người mua trả tiền trước Quý {configMain.quarter}/{configMain.year}", StaticVal._lVin.ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.NguoiMua).ToList(),
                        name = "Người mua trả tiền trước",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.TangTruong).ToList(),
                        name = "Tăng trưởng người mua",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
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

        public async Task<Stream> Chart_VIN_NoTrenVonChu()
        {
            try
            {
                var configMain = _configMainRepo.GetAll().First();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
                if (!lFinancial.Any())
                    return null;

                var lResult = new List<HighChart_NoTrenVonChu>();
                foreach (var item in StaticVal._lVin)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lResult.Add(new HighChart_NoTrenVonChu
                        {
                            d = int.Parse($"{configMain.year}{configMain.quarter}"),
                            s = item
                        });
                        continue;
                    }

                    double noTrenVonChu = 0;
                    if (cur.eq != 0)
                    {
                        var sign = cur.eq >= cur.debt;

                        noTrenVonChu = Math.Abs(Math.Round(100 * (cur.debt / cur.eq), 1));
                        if (!sign)
                        {
                            noTrenVonChu = -noTrenVonChu;
                        }
                    }

                    //tang truong tin dung, room tin dung
                    lResult.Add(new HighChart_NoTrenVonChu
                    {
                        s = cur.s,
                        d = cur.d,
                        VonChu = cur.eq,
                        No = cur.debt,
                        NoTrenVonChu = noTrenVonChu
                    });
                }

                var basicColumn = new HighchartBasicColumn($"Nợ trên vốn chủ sở hữu Quý {configMain.quarter}/{configMain.year}", StaticVal._lVin.ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.VonChu).ToList(),
                        name = "Vốn chủ sở hữu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.No).ToList(),
                        name = "Nợ",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.NoTrenVonChu).ToList(),
                        name = "Nợ trên vố chủ sở hữu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
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
