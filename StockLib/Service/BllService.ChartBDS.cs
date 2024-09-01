using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        private async Task<IEnumerable<string>> GetList(IEnumerable<string> lInput) 
        {
            try
            {
                var time = GetCurrentTime();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1));
                lFinancial = lFinancial.Where(x => lInput.Contains(x.s))
                                        .Where(x => x.bp >= 500 && x.inv >= 500).ToList();

                var lFinancialPrev1 = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter()));
                var lFinancialPrev2 = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter()));
                var lFinancialPrev3 = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter().GetPrevQuarter()));
                var lFinancialPrev4 = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter().GetPrevQuarter().GetPrevQuarter()));
                var lFinancialPrev5 = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter().GetPrevQuarter().GetPrevQuarter().GetPrevQuarter()));

                var lLast = new List<(string, double, double, double, double, double, int)>();
                var lClean1 = DetectSymbol(lFinancial, lFinancialPrev1);
                var lClean2 = DetectSymbol(lFinancialPrev1, lFinancialPrev2);
                var lClean3 = DetectSymbol(lFinancialPrev2, lFinancialPrev3);
                var lClean4 = DetectSymbol(lFinancialPrev3, lFinancialPrev4);
                var lClean5 = DetectSymbol(lFinancialPrev4, lFinancialPrev5);

                var lClean11 = lFinancial.Where(x => lClean1.Any(y => y.Item1 == x.s)).ToList();
                var lClean12 = lFinancial.Where(x => lClean2.Any(y => y.Item1 == x.s)).ToList();
                var lClean13 = lFinancial.Where(x => lClean3.Any(y => y.Item1 == x.s)).ToList();
                var lClean14 = lFinancial.Where(x => lClean4.Any(y => y.Item1 == x.s)).ToList();
                var lClean15 = lFinancial.Where(x => lClean5.Any(y => y.Item1 == x.s)).ToList();

                foreach (var item in lFinancial)
                {
                    var count = 0;

                    if (lClean11.Any(x => x.s == item.s))
                        count++;

                    if (lClean12.Any(x => x.s == item.s))
                        count++;

                    if (lClean13.Any(x => x.s == item.s))
                        count++;

                    if (lClean14.Any(x => x.s == item.s))
                        count++;

                    if (lClean15.Any(x => x.s == item.s))
                        count++;

                    if (count >= 3)
                    {
                        var clean1 = lClean1.FirstOrDefault(x => x.Item1 == item.s);
                        var clean2 = lClean2.FirstOrDefault(x => x.Item1 == item.s);
                        var clean3 = lClean3.FirstOrDefault(x => x.Item1 == item.s);
                        var clean4 = lClean4.FirstOrDefault(x => x.Item1 == item.s);
                        var clean5 = lClean5.FirstOrDefault(x => x.Item1 == item.s);
                        lLast.Add((item.s, clean1.Item2, clean2.Item2, clean3.Item2, clean4.Item2, clean5.Item2, count));
                    }
                }

                return lLast.OrderByDescending(x => x.Item7).Take(15).Select(x => x.Item1);

                List<(string, double)> DetectSymbol(List<Financial_BDS> lCur, List<Financial_BDS> lPrev)
                {
                    var lClean = new List<(string, double)>();
                    foreach (var item in lCur)
                    {
                        var itemPrev = lPrev.FirstOrDefault(x => x.s == item.s);
                        if (itemPrev is null)
                            continue;

                        var rate = Math.Round(item.bp * 100 / itemPrev.bp, 1);
                        if (rate >= 105)
                        {
                            lClean.Add((item.s, rate));
                        }
                    }


                    return lClean;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BatDongSan|EXCEPTION| {ex.Message}");
            }

            return null;
        }
        public async Task<List<Stream>> Chart_BatDongSan(IEnumerable<string> lInput)
        {
            var lMaCK = GetList(lInput);
            if (lMaCK is null)
                return null;

            //Người mua, tồn kho, tỉ lệ người mua/tồn kho



            return null;
        }

        //public async Task<Stream> Chart_BDS_NguoiMua(IEnumerable<string> lInput)
        //{
        //    try
        //    {
        //        var lBDS = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
        //                               ).Take(15).ToList();
        //        lBDS.Remove("KSF");
        //        lBDS.Remove("VPI");
        //        lBDS.Add("DPG");
        //        lBDS.Add("NTL");

        //        var configMain = _configMainRepo.GetAll().First();
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
        //        if (!lFinancial.Any())
        //            return null;

        //        var yearPrev = configMain.year;
        //        var quarterPrev = configMain.quarter;
        //        if (configMain.quarter > 1)
        //        {
        //            quarterPrev--;
        //        }
        //        else
        //        {
        //            quarterPrev = 4;
        //            yearPrev--;
        //        }
        //        var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

        //        var lNguoiMua = new List<double>();
        //        var lTangTruong = new List<double>();
        //        foreach (var item in lBDS)
        //        {
        //            var cur = lFinancial.FirstOrDefault(x => x.s == item);
        //            if (cur is null)
        //            {
        //                lNguoiMua.Add(0);
        //                lTangTruong.Add(0);
        //                continue;
        //            }

        //            double tangTruong = 0;
        //            var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
        //            if (prev is not null && prev.bp > 0)
        //            {
        //                tangTruong = Math.Round(100 * (-1 + cur.bp / prev.bp), 1);
        //            }

        //            //tang truong tin dung, room tin dung
        //            lNguoiMua.Add(cur.bp);
        //            lTangTruong.Add(tangTruong);
        //        }

        //        var lSeries = new List<HighChartSeries_BasicColumn>
        //        {
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lNguoiMua,
        //                name = "Người mua trả tiền trước",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#012060"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lTangTruong,
        //                name = "Tăng trưởng người mua",
        //                type = "spline",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
        //                color = "#C00000",
        //                yAxis = 1,
        //            }
        //        };

        //        return await Chart_BasicBase($"Người mua trả tiền trước Quý {configMain.quarter}/{configMain.year} (QoQ)", lBDS, lSeries);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_BDS_NguoiMua|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}

        //public async Task<Stream> Chart_BDS_DoanhThu_LoiNhuan(IEnumerable<string> lInput)
        //{
        //    try
        //    {
        //        var lMaCK = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
        //                                ).Take(15).ToList();
        //        lMaCK.Remove("KSF");
        //        lMaCK.Remove("VPI");
        //        lMaCK.Add("DPG");
        //        lMaCK.Add("NTL");

        //        var configMain = _configMainRepo.GetAll().First();
        //        var d = int.Parse($"{configMain.year}{configMain.quarter}");
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, d));
        //        if (!lFinancial.Any())
        //            return null;

        //        var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year - 1}{configMain.quarter}")));
        //        return await Chart_DoanhThuBase(lMaCK, configMain, d, lFinancial.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), null, isTangTruongLoiNhuan: true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_BDS_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}

        //public async Task<Stream> Chart_BDS_TonKho(IEnumerable<string> lInput)
        //{
        //    try
        //    {
        //        var lBDS = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
        //                               ).Take(15).ToList();
        //        lBDS.Remove("KSF");
        //        lBDS.Remove("VPI");
        //        lBDS.Add("DPG");
        //        lBDS.Add("NTL");

        //        var configMain = _configMainRepo.GetAll().First();
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
        //        if (!lFinancial.Any())
        //            return null;

        //        var yearPrev = configMain.year;
        //        var quarterPrev = configMain.quarter;
        //        if (configMain.quarter > 1)
        //        {
        //            quarterPrev--;
        //        }
        //        else
        //        {
        //            quarterPrev = 4;
        //            yearPrev--;
        //        }
        //        var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

        //        var lTonKho = new List<double>();
        //        var lTangTruongTonKho = new List<double>();
        //        foreach (var item in lBDS)
        //        {
        //            var cur = lFinancial.FirstOrDefault(x => x.s == item);
        //            if (cur is null)
        //            {
        //                lTonKho.Add(0);
        //                lTangTruongTonKho.Add(0);
        //                continue;
        //            }

        //            double tangTruong = 0;
        //            var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
        //            if (prev is not null && prev.inv > 0)
        //            {
        //                tangTruong = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
        //            }

        //            //tang truong tin dung, room tin dung
        //            lTonKho.Add(cur.inv);
        //            lTangTruongTonKho.Add(tangTruong);
        //        }
        //        var lSeries = new List<HighChartSeries_BasicColumn>
        //        {
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lTonKho,
        //                name = "Tồn kho",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#012060"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lTangTruongTonKho,
        //                name = "Tăng trưởng tồn kho",
        //                type = "spline",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
        //                color = "#C00000",
        //                yAxis = 1,
        //            }
        //        };

        //        return await Chart_BasicBase($"Tồn kho Quý {configMain.quarter}/{configMain.year} (QoQ)", lBDS, lSeries);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_BDS_TonKho|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}



        //public async Task<Stream> Chart_BDS_NoTrenVonChu(IEnumerable<string> lInput)
        //{
        //    try
        //    {
        //        var lBDS = lInput.Where(x => !StaticVal._lKCN.Contains(x) && !StaticVal._lVin.Contains(x)
        //                               ).Take(15).ToList();
        //        lBDS.Remove("KSF");
        //        lBDS.Remove("VPI");
        //        lBDS.Add("DPG");
        //        lBDS.Add("NTL");

        //        var configMain = _configMainRepo.GetAll().First();
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
        //        if (!lFinancial.Any())
        //            return null;

        //        var lVonChu = new List<double>();
        //        var lNo = new List<double>();
        //        var lNoTrenVonChu = new List<double>();
        //        foreach (var item in lBDS)
        //        {
        //            var cur = lFinancial.FirstOrDefault(x => x.s == item);
        //            if (cur is null)
        //            {
        //                lVonChu.Add(0);
        //                lNo.Add(0);
        //                lNoTrenVonChu.Add(0);
        //                continue;
        //            }

        //            double noTrenVonChu = 0;
        //            if (cur.eq != 0)
        //            {
        //                var sign = cur.eq >= cur.debt;

        //                noTrenVonChu = Math.Abs(Math.Round(100 * (cur.debt / cur.eq), 1));
        //                if (!sign)
        //                {
        //                    noTrenVonChu = -noTrenVonChu;
        //                }
        //            }

        //            //
        //            lVonChu.Add(cur.eq);
        //            lNo.Add(cur.debt);
        //            lNoTrenVonChu.Add(noTrenVonChu);
        //        }

        //        var lSeries = new List<HighChartSeries_BasicColumn>
        //        {
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lVonChu,
        //                name = "Vốn chủ sở hữu",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#012060"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lNo,
        //                name = "Nợ",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#C00000"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lNoTrenVonChu,
        //                name = "Nợ trên vố chủ sở hữu",
        //                type = "spline",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
        //                color = "#C00000",
        //                yAxis = 1,
        //            }
        //        };

        //        return await Chart_BasicBase($"Nợ trên vốn chủ sở hữu Quý {configMain.quarter}/{configMain.year}", lBDS, lSeries);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_BDS_NoTrenVonChu|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}


        //public async Task<Stream> Chart_VIN_DoanhThu_LoiNhuan()
        //{
        //    try
        //    {
        //        var configMain = _configMainRepo.GetAll().First();
        //        var d = int.Parse($"{configMain.year}{configMain.quarter}");
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, d));
        //        if (!lFinancial.Any())
        //            return null;

        //        var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year-1}{configMain.quarter}")));
        //        return await Chart_DoanhThuBase(StaticVal._lVin, configMain, d, lFinancial.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), null, isTangTruongLoiNhuan: true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_VIN_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}

        //public async Task<Stream> Chart_VIN_TonKho()
        //{
        //    try
        //    {
        //        var configMain = _configMainRepo.GetAll().First();
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
        //        if (!lFinancial.Any())
        //            return null;

        //        var yearPrev = configMain.year;
        //        var quarterPrev = configMain.quarter;
        //        if (configMain.quarter > 1)
        //        {
        //            quarterPrev--;
        //        }
        //        else
        //        {
        //            quarterPrev = 4;
        //            yearPrev--;
        //        }
        //        var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

        //        var lTonKho = new List<double>();
        //        var lTangTruongTonKho = new List<double>();
        //        foreach (var item in StaticVal._lVin)
        //        {
        //            var cur = lFinancial.FirstOrDefault(x => x.s == item);
        //            if (cur is null)
        //            {
        //                lTonKho.Add(0);
        //                lTangTruongTonKho.Add(0);
        //                continue;
        //            }

        //            double tangTruong = 0;
        //            var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
        //            if (prev is not null && prev.inv > 0)
        //            {
        //                tangTruong = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
        //            }

        //            //tang truong tin dung, room tin dung
        //            lTonKho.Add(cur.inv);
        //            lTangTruongTonKho.Add(tangTruong);
        //        }
        //        var lSeries = new List<HighChartSeries_BasicColumn>
        //        {
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lTonKho,
        //                name = "Tồn kho",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#012060"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lTangTruongTonKho,
        //                name = "Tăng trưởng tồn kho",
        //                type = "spline",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
        //                color = "#C00000",
        //                yAxis = 1,
        //            }
        //        };
        //        return await Chart_BasicBase($"Tồn kho Quý {configMain.quarter}/{configMain.year} (QoQ)", StaticVal._lVin, lSeries);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_VIN_TonKho|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}

        //public async Task<Stream> Chart_VIN_NguoiMua()
        //{
        //    try
        //    {
        //        var configMain = _configMainRepo.GetAll().First();
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
        //        if (!lFinancial.Any())
        //            return null;

        //        var yearPrev = configMain.year;
        //        var quarterPrev = configMain.quarter;
        //        if (configMain.quarter > 1)
        //        {
        //            quarterPrev--;
        //        }
        //        else
        //        {
        //            quarterPrev = 4;
        //            yearPrev--;
        //        }
        //        var lFinancialPrev = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

        //        var lNguoiMua = new List<double>();
        //        var lTangTruong = new List<double>();
        //        foreach (var item in StaticVal._lVin)
        //        {
        //            var cur = lFinancial.FirstOrDefault(x => x.s == item);
        //            if (cur is null)
        //            {
        //                lNguoiMua.Add(0);
        //                lTangTruong.Add(0);
        //                continue;
        //            }

        //            double tangTruong = 0;
        //            var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
        //            if (prev is not null && prev.bp > 0)
        //            {
        //                tangTruong = Math.Round(100 * (-1 + cur.bp / prev.bp), 1);
        //            }

        //            //tang truong tin dung, room tin dung
        //            lNguoiMua.Add(cur.bp);
        //            lTangTruong.Add(tangTruong);
        //        }

        //        var lSeries = new List<HighChartSeries_BasicColumn>
        //        {
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lNguoiMua,
        //                name = "Người mua trả tiền trước",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#012060"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lTangTruong,
        //                name = "Tăng trưởng người mua",
        //                type = "spline",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
        //                color = "#C00000",
        //                yAxis = 1,
        //            }
        //        };

        //        return await Chart_BasicBase($"Người mua trả tiền trước Quý {configMain.quarter}/{configMain.year} (QoQ)", StaticVal._lVin, lSeries);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_VIN_NguoiMua|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}

        //public async Task<Stream> Chart_VIN_NoTrenVonChu()
        //{
        //    try
        //    {
        //        var configMain = _configMainRepo.GetAll().First();
        //        var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, int.Parse($"{configMain.year}{configMain.quarter}")));
        //        if (!lFinancial.Any())
        //            return null;

        //        var lVonChu = new List<double>();
        //        var lNo = new List<double>();
        //        var lNoTrenVonChu = new List<double>();
        //        foreach (var item in StaticVal._lVin)
        //        {
        //            var cur = lFinancial.FirstOrDefault(x => x.s == item);
        //            if (cur is null)
        //            {
        //                lVonChu.Add(0);
        //                lNo.Add(0);
        //                lNoTrenVonChu.Add(0);
        //                continue;
        //            }

        //            double noTrenVonChu = 0;
        //            if (cur.eq != 0)
        //            {
        //                var sign = cur.eq >= cur.debt;

        //                noTrenVonChu = Math.Abs(Math.Round(100 * (cur.debt / cur.eq), 1));
        //                if (!sign)
        //                {
        //                    noTrenVonChu = -noTrenVonChu;
        //                }
        //            }

        //            //tang truong tin dung, room tin dung
        //            lVonChu.Add(cur.eq);
        //            lNo.Add(cur.debt);
        //            lNoTrenVonChu.Add(noTrenVonChu);
        //        }

        //        var lSeries = new List<HighChartSeries_BasicColumn>
        //        {
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lVonChu,
        //                name = "Vốn chủ sở hữu",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#012060"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lNo,
        //                name = "Nợ",
        //                type = "column",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
        //                color = "#C00000"
        //            },
        //            new HighChartSeries_BasicColumn
        //            {
        //                data = lNoTrenVonChu,
        //                name = "Nợ trên vố chủ sở hữu",
        //                type = "spline",
        //                dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
        //                color = "#C00000",
        //                yAxis = 1,
        //            }
        //        };

        //        return await Chart_BasicBase($"Nợ trên vốn chủ sở hữu Quý {configMain.quarter}/{configMain.year}", StaticVal._lVin, lSeries);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"BllService.Chart_VIN_NoTrenVonChu|EXCEPTION| {ex.Message}");
        //    }
        //    return null;
        //}
    }
}
