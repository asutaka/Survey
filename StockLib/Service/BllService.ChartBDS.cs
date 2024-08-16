using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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
                var d = int.Parse($"{configMain.year}{configMain.quarter}");
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, d));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year - 1}{configMain.quarter}")));
                return await Chart_DoanhThu_LoiNhuanBase(lBDS, configMain, d, lFinancial.Select(x => (x.s, x.rv, x.pf)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
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

                var lTonKho = new List<double>();
                var lTangTruongTonKho = new List<double>();
                foreach (var item in lBDS)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lTonKho.Add(0);
                        lTangTruongTonKho.Add(0);
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.inv > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lTonKho.Add(cur.inv);
                    lTangTruongTonKho.Add(tangTruong);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTonKho,
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongTonKho,
                        name = "Tăng trưởng tồn kho",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Tồn kho Quý {configMain.quarter}/{configMain.year} (QoQ)", lBDS, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_TonKho|EXCEPTION| {ex.Message}");
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

                var lNguoiMua = new List<double>();
                var lTangTruong = new List<double>();
                foreach (var item in lBDS)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lNguoiMua.Add(0);
                        lTangTruong.Add(0);
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.bp > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.bp / prev.bp), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lNguoiMua.Add(cur.bp);
                    lTangTruong.Add(tangTruong);
                }

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lNguoiMua,
                        name = "Người mua trả tiền trước",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong,
                        name = "Tăng trưởng người mua",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Người mua trả tiền trước Quý {configMain.quarter}/{configMain.year} (QoQ)", lBDS, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_NguoiMua|EXCEPTION| {ex.Message}");
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

                var lVonChu = new List<double>();
                var lNo = new List<double>();
                var lNoTrenVonChu = new List<double>();
                foreach (var item in lBDS)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lVonChu.Add(0);
                        lNo.Add(0);
                        lNoTrenVonChu.Add(0);
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

                    //
                    lVonChu.Add(cur.eq);
                    lNo.Add(cur.debt);
                    lNoTrenVonChu.Add(noTrenVonChu);
                }

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lVonChu,
                        name = "Vốn chủ sở hữu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lNo,
                        name = "Nợ",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lNoTrenVonChu,
                        name = "Nợ trên vố chủ sở hữu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Nợ trên vốn chủ sở hữu Quý {configMain.quarter}/{configMain.year}", lBDS, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_NoTrenVonChu|EXCEPTION| {ex.Message}");
            }
            return null;
        }


        public async Task<Stream> Chart_VIN_DoanhThu_LoiNhuan()
        {
            try
            {
                var configMain = _configMainRepo.GetAll().First();
                var d = int.Parse($"{configMain.year}{configMain.quarter}");
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, d));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year-1}{configMain.quarter}")));
                return await Chart_DoanhThu_LoiNhuanBase(StaticVal._lVin, configMain, d, lFinancial.Select(x => (x.s, x.rv, x.pf)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_VIN_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
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

                var lTonKho = new List<double>();
                var lTangTruongTonKho = new List<double>();
                foreach (var item in StaticVal._lVin)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lTonKho.Add(0);
                        lTangTruongTonKho.Add(0);
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.inv > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lTonKho.Add(cur.inv);
                    lTangTruongTonKho.Add(tangTruong);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTonKho,
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongTonKho,
                        name = "Tăng trưởng tồn kho",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };
                return await Chart_BasicBase($"Tồn kho Quý {configMain.quarter}/{configMain.year} (QoQ)", StaticVal._lVin, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_VIN_TonKho|EXCEPTION| {ex.Message}");
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

                var lNguoiMua = new List<double>();
                var lTangTruong = new List<double>();
                foreach (var item in StaticVal._lVin)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lNguoiMua.Add(0);
                        lTangTruong.Add(0);
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.bp > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.bp / prev.bp), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lNguoiMua.Add(cur.bp);
                    lTangTruong.Add(tangTruong);
                }

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lNguoiMua,
                        name = "Người mua trả tiền trước",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong,
                        name = "Tăng trưởng người mua",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Người mua trả tiền trước Quý {configMain.quarter}/{configMain.year} (QoQ)", StaticVal._lVin, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_VIN_NguoiMua|EXCEPTION| {ex.Message}");
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

                var lVonChu = new List<double>();
                var lNo = new List<double>();
                var lNoTrenVonChu = new List<double>();
                foreach (var item in StaticVal._lVin)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lVonChu.Add(0);
                        lNo.Add(0);
                        lNoTrenVonChu.Add(0);
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
                    lVonChu.Add(cur.eq);
                    lNo.Add(cur.debt);
                    lNoTrenVonChu.Add(noTrenVonChu);
                }

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lVonChu,
                        name = "Vốn chủ sở hữu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lNo,
                        name = "Nợ",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lNoTrenVonChu,
                        name = "Nợ trên vố chủ sở hữu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Nợ trên vốn chủ sở hữu Quý {configMain.quarter}/{configMain.year}", StaticVal._lVin, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_VIN_NoTrenVonChu|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
