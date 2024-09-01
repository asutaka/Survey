using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        private async Task<IEnumerable<string>> GetList5Quarter(IEnumerable<string> lInput) 
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

                return lLast.OrderByDescending(x => x.Item7).Take(10).Select(x => x.Item1);

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
                _logger.LogError($"BllService.GetList|EXCEPTION| {ex.Message}");
            }

            return null;
        }
        private async Task<IEnumerable<string>> GetList1Quarter(IEnumerable<string> lInput)
        {
            try
            {
                var time = GetCurrentTime();
                var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1));
                lFinancial = lFinancial.Where(x => lInput.Contains(x.s))
                                        .Where(x => x.bp >= 500 && x.inv >= 500).ToList();

                var lFinancialPrev1 = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter()));

                var lLast = new List<(string, double)>();
                var lClean1 = DetectSymbol(lFinancial, lFinancialPrev1);

                var lClean11 = lFinancial.Where(x => lClean1.Any(y => y.Item1 == x.s)).ToList();

                foreach (var item in lFinancial)
                {
                    var clean1 = lClean1.FirstOrDefault(x => x.Item1 == item.s);
                    lLast.Add((item.s, clean1.Item2));
                }

                return lLast.OrderByDescending(x => x.Item2).Take(5).Select(x => x.Item1);

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
                _logger.LogError($"BllService.GetList|EXCEPTION| {ex.Message}");
            }

            return null;
        }
        public async Task<List<Stream>> Chart_BatDongSan(IEnumerable<string> lInput)
        {
            var lOutput = new List<Stream>();
            var lMaCK5Quarter = await GetList5Quarter(lInput);
            if(lMaCK5Quarter is null)
                lMaCK5Quarter= new List<string>();

            var lMaCK1Quarter = await GetList1Quarter(lInput);
            if(lMaCK1Quarter is null)
                lMaCK1Quarter= new List<string>();

            if (!lMaCK5Quarter.Any()
                && !lMaCK1Quarter.Any())
                return null;

            //Người mua, tồn kho, tỉ lệ người mua/tồn kho
            var time = GetCurrentTime();
            var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.d, time.Item1));
            if (!lFinancial.Any())
                return null;

            var lMaCKSort = StaticVal._lStock.Where(x => lMaCK5Quarter.Contains(x.s) || lMaCK1Quarter.Contains(x.s)).OrderBy(x => x.rank).Select(x => x.s);
            var lFinancialFilter = new List<Financial_BDS>();
            foreach (var item in lMaCKSort)
            {
                var first = lFinancial.FirstOrDefault(x => x.s == item);
                lFinancialFilter.Add(first);
            }

            var lSeries = new List<HighChartSeries_BasicColumn>
            {
                new HighChartSeries_BasicColumn
                {
                    data = lFinancialFilter.Select(x => x.inv),
                    name = "Tồn kho",
                    type = "column",
                    dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                    color = "#012060"
                },
                 new HighChartSeries_BasicColumn
                {
                    data = lFinancialFilter.Select(x => x.bp),
                    name = "Người mua trả tiền trước",
                    type = "column",
                    dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                    color = "#C00000"
                },
                new HighChartSeries_BasicColumn
                {
                    data = lFinancialFilter.Select(x => Math.Round(x.bp*100/x.inv, 1)),
                    name = "Tỉ lệ người mua/tồn kho",
                    type = "spline",
                    dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                    color = "#ffbf00",
                    yAxis = 1,
                }
            };

            var streamBDS = await Chart_BasicBase($"Nhóm Ngành Bất Động Sản", lMaCKSort.ToList(), lSeries);
            lOutput.Add(streamBDS);

            return lOutput;
        }

        public async Task<List<Stream>> Chart_BatDongSan(string code)
        {
            var lFinancial = _bdsRepo.GetByFilter(Builders<Financial_BDS>.Filter.Eq(x => x.s, code));
            if (!lFinancial.Any())
                return null;

            var lOutput = new List<Stream>();

            lFinancial = lFinancial.OrderBy(x => x.d).ToList();
            var streamNguoiMua = await Chart_BDS_NguoiMua(lFinancial, code);
            var streamTonKho = await Chart_BDS_TonKho(lFinancial, code);
            var streamDoanhThu = await Chart_BDS_DoanhThu_LoiNhuan(lFinancial, code);
            lOutput.Add(streamNguoiMua);
            lOutput.Add(streamTonKho);
            lOutput.Add(streamDoanhThu);
            return lOutput;
        }

        public async Task<Stream> Chart_BDS_NguoiMua(List<Financial_BDS> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                var lTangTruong = new List<double>();
                foreach (var item in lFinancial)
                {
                    double tangTruong = 0;
                    var prevQuarter = item.d.GetPrevQuarter();
                    var prev = lFinancial.FirstOrDefault(x => x.d == prevQuarter);
                    if (prev is not null && prev.bp > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + item.bp / prev.bp), 1);
                    }

                    lTangTruong.Add(tangTruong);
                }
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.bp),
                        name = "Người mua trả tiền trước",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng người mua",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Người mua trả tiền trước Quý {time.Item3}/{time.Item2} (QoQoY)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_NguoiMua|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        public async Task<Stream> Chart_BDS_TonKho(List<Financial_BDS> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                var lTangTruong = new List<double>();
                foreach (var item in lFinancial)
                {
                    double tangTruong = 0;
                    var prevQuarter = item.d.GetPrevQuarter();
                    var prev = lFinancial.FirstOrDefault(x => x.d == prevQuarter);
                    if (prev is not null && prev.inv > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + item.inv / prev.inv), 1);
                    }

                    lTangTruong.Add(tangTruong);
                }
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.inv),
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng tồn kho",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Tồn kho Quý {time.Item3}/{time.Item2} (QoQoY)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_TonKho|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        public async Task<Stream> Chart_BDS_DoanhThu_LoiNhuan(List<Financial_BDS> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                var lTangTruong = new List<double>();
                foreach (var item in lFinancial)
                {
                    double tangTruong = 0;
                    var prevQuarter = item.d.GetYoyQuarter();
                    var prev = lFinancial.FirstOrDefault(x => x.d == prevQuarter);
                    if (prev is not null && prev.pf != 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + item.pf / prev.pf), 1);
                        if(item.pf > prev.pf)
                        {
                            tangTruong = Math.Abs(tangTruong);
                        }
                        if(tangTruong >= StaticVal._MaxRate)
                        {
                            tangTruong = StaticVal._MaxRate;
                        }
                        if(tangTruong <= -StaticVal._MaxRate)
                        {
                            tangTruong = -StaticVal._MaxRate;
                        }
                    }

                    lTangTruong.Add(tangTruong);
                }
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.rv),
                        name = "Doanh thu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                     new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.pf),
                        name = "Lợi nhuận",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng lợi nhuận",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Doanh thu, Lợi nhuận Quý {time.Item3}/{time.Item2} (QoQ)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        //public async Task<Stream> Chart_BDS_DoanhThu_LoiNhuan(List<Financial_BDS> lFinancial, string code)
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
