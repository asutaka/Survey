using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_Oto(IEnumerable<string> lInput)
        {
            try
            {
                var lOutput = new List<Stream>();
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();
                var lFinancial = _otoRepo.GetByFilter(Builders<Financial_Oto>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var streamTonkho = await Chart_Oto_TonKho(lInput, lFinancial);
                lOutput.Add(streamTonkho);

                var streamNo = await Chart_Oto_NoTaiChinh(lInput, lFinancial);
                lOutput.Add(streamNo);

                var streamNK = await Chart_NhapKhau_Oto();
                lOutput.Add(streamNK);
                return lOutput;

            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Oto|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_Oto_TonKho(IEnumerable<string> lInput, List<Financial_Oto> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();
                var lOrderBy = new List<Financial_Oto>();
                foreach (var item in lInput)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                        continue;
                    lOrderBy.Add(cur);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lOrderBy.Select(x => x.eq).ToList(),
                        name = "Vốn chủ sở hữu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lOrderBy.Select(x => x.inv).ToList(),
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lOrderBy.Select(x => Math.Round(x.inv * 100/ x.eq, 1)).ToList(),
                        name = "Tồn kho trên vốn chủ sở hữu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Tồn kho Quý {time.Item3}/{time.Item2}", lInput.ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Oto_TonKho|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_Oto_NoTaiChinh(IEnumerable<string> lInput, List<Financial_Oto> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();
                var lOrderBy = new List<Financial_Oto>();
                foreach (var item in lInput)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                        continue;
                    lOrderBy.Add(cur);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lOrderBy.Select(x => x.eq).ToList(),
                        name = "Vốn chủ sở hữu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lOrderBy.Select(x => x.debt).ToList(),
                        name = "Nợ tài chính",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lOrderBy.Select(x => Math.Round(x.debt * 100/ x.eq, 1)).ToList(),
                        name = "Nợ trên vốn chủ sở hữu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Nợ trên vốn chủ sở hữu Quý {time.Item3}/{time.Item2}", lInput.ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Oto_No|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<List<Stream>> Chart_Oto(string code)
        {
            var lFinancial = _otoRepo.GetByFilter(Builders<Financial_Oto>.Filter.Eq(x => x.s, code));
            if (!lFinancial.Any())
                return null;

            var lOutput = new List<Stream>();

            lFinancial = lFinancial.OrderBy(x => x.d).ToList();
            var streamTonKho = await Chart_Oto_TonKho(lFinancial, code);
            var streamNoTaiChinh = await Chart_Oto_NoTaiChinh(lFinancial, code);
            var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), code);
            var streamNK = await Chart_NhapKhau_Oto();
            lOutput.Add(streamTonKho);
            lOutput.Add(streamNoTaiChinh);
            lOutput.Add(streamDoanhThu);
            lOutput.Add(streamNK);
            return lOutput;
        }

        private async Task<Stream> Chart_Oto_TonKho(List<Financial_Oto> lFinancial, string code)
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
                _logger.LogError($"BllService.Chart_Oto_TonKho|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_Oto_NoTaiChinh(List<Financial_Oto> lFinancial, string code)
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
                    if (prev is not null && prev.debt > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + item.debt / prev.debt), 1);
                    }

                    lTangTruong.Add(tangTruong);
                }
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.debt),
                        name = "Nợ tài chính",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng nợ",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Nợ Quý {time.Item3}/{time.Item2} (QoQoY)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Oto_NoTaiChinh|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_NhapKhau_Oto()
        {
            try
            {
                var lOto9cho_NK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)EHaiQuan.Oto9Cho_NK)).OrderBy(x => x.d);
                var lOtotai_NK = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)EHaiQuan.OtoVanTai_NK)).OrderBy(x => x.d);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lOto9cho_NK.TakeLast(25).Select(x => x.va),
                        name = "Giá trị nhập khẩu ô tô dưới 9 chỗ",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060",
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lOtotai_NK.TakeLast(25).Select(x => x.va),
                        name = "Giá trị nhập khẩu ô tô tải",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                    }
                };

                if (lOto9cho_NK.Sum(x => x.price) > 0)
                {
                    lSeries.Add(new HighChartSeries_BasicColumn
                    {
                        data = lOto9cho_NK.TakeLast(25).Select(x => x.price),
                        name = "Giá nhập khẩu ô tô dưới 9 chỗ",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#012060",
                        yAxis = 1
                    });
                }

                if (lOtotai_NK.Sum(x => x.price) > 0)
                {
                    lSeries.Add(new HighChartSeries_BasicColumn
                    {
                        data = lOtotai_NK.TakeLast(25).Select(x => x.price),
                        name = "Giá nhập khẩu ô tô tải",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                        yAxis = 1
                    });
                }

                return await Chart_BasicBase($"Nhập khẩu - Thống kê nửa tháng", lOto9cho_NK.TakeLast(25).Select(x => x.d.GetNameHaiQuan()).ToList(), lSeries, "giá trị: triệu USD", "giá trị: USD");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NhapKhau|EXCEPTION| {ex.Message}");
            }

            return null;
        }
    }
}
