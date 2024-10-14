using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_BatDongSan(IEnumerable<string> lInput)
        {
            var lOutput = new List<Stream>();
            var lMaCK5Quarter = GetList5Quarter(lInput);
            if(lMaCK5Quarter is null)
                lMaCK5Quarter= new List<string>();

            var lMaCK1Quarter = GetList1Quarter(lInput);
            if(lMaCK1Quarter is null)
                lMaCK1Quarter= new List<string>();

            if (!lMaCK5Quarter.Any()
                && !lMaCK1Quarter.Any())
                return null;

            //Người mua, tồn kho, tỉ lệ người mua/tồn kho
            var time = GetCurrentTime();
            var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1));
            if (!lFinancial.Any())
                return null;

            var lMaCKSort = StaticVal._lStock.Where(x => lMaCK5Quarter.Contains(x.s) || lMaCK1Quarter.Contains(x.s)).OrderBy(x => x.rank).Select(x => x.s);
            var lFinancialFilter = new List<Financial>();
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
        private IEnumerable<string> GetList5Quarter(IEnumerable<string> lInput)
        {
            try
            {
                var time = GetCurrentTime();
                var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1));
                lFinancial = lFinancial.Where(x => lInput.Contains(x.s))
                                        .Where(x => x.bp >= 500 && x.inv >= 500).ToList();

                var lFinancialPrev1 = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter()));
                var lFinancialPrev2 = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter()));
                var lFinancialPrev3 = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter().GetPrevQuarter()));
                var lFinancialPrev4 = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter().GetPrevQuarter().GetPrevQuarter()));
                var lFinancialPrev5 = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter().GetPrevQuarter().GetPrevQuarter().GetPrevQuarter().GetPrevQuarter()));

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

                List<(string, double)> DetectSymbol(List<Financial> lCur, List<Financial> lPrev)
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
        private IEnumerable<string> GetList1Quarter(IEnumerable<string> lInput)
        {
            try
            {
                var time = GetCurrentTime();
                var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1));
                lFinancial = lFinancial.Where(x => lInput.Contains(x.s))
                                        .Where(x => x.bp >= 500 && x.inv >= 500).ToList();

                var lFinancialPrev1 = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1.GetPrevQuarter()));

                var lLast = new List<(string, double)>();
                var lClean1 = DetectSymbol(lFinancial, lFinancialPrev1);

                var lClean11 = lFinancial.Where(x => lClean1.Any(y => y.Item1 == x.s)).ToList();

                foreach (var item in lFinancial)
                {
                    var clean1 = lClean1.FirstOrDefault(x => x.Item1 == item.s);
                    lLast.Add((item.s, clean1.Item2));
                }

                return lLast.OrderByDescending(x => x.Item2).Take(5).Select(x => x.Item1);

                List<(string, double)> DetectSymbol(List<Financial> lCur, List<Financial> lPrev)
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

        private async Task<List<Stream>> Chart_BatDongSan(string code)
        {
            var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
            if (!lFinancial.Any())
                return null;

            var lOutput = new List<Stream>();

            lFinancial = lFinancial.OrderBy(x => x.d).ToList();
            var streamNguoiMua = await Chart_BDS_NguoiMua(lFinancial, code);
            var streamTonKho = await Chart_BDS_TonKho(lFinancial, code);
            var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf}).ToList(), code);
            lOutput.Add(streamNguoiMua);
            lOutput.Add(streamTonKho);
            lOutput.Add(streamDoanhThu);
            return lOutput;
        }
        private async Task<Stream> Chart_BDS_NguoiMua(List<Financial> lFinancial, string code)
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
        private async Task<Stream> Chart_BDS_TonKho(List<Financial> lFinancial, string code)
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
    }
}
