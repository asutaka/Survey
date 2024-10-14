using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_BanLe(IEnumerable<string> lInput)
        {
            try
            {
                var lOutput = new List<Stream>();
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();
                var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var streamTonkho = await Chart_BanLe_TonKho(lInput, lFinancial);
                lOutput.Add(streamTonkho);

                var streamNo = await Chart_BanLe_NoTaiChinh(lInput, lFinancial);
                lOutput.Add(streamNo);

                var streamThongKe = await Chart_ThongKe_BanLe();
                lOutput.Add(streamThongKe);
                return lOutput;

            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BanLe|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_BanLe_TonKho(IEnumerable<string> lInput, List<Financial> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();
                var lOrderBy = new List<Financial>();
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
                _logger.LogError($"BllService.Chart_BanLe_TonKho|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_BanLe_NoTaiChinh(IEnumerable<string> lInput, List<Financial> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();
                var lOrderBy = new List<Financial>();
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
                _logger.LogError($"BllService.Chart_BanLe_No|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<List<Stream>> Chart_BanLe(string code)
        {
            var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
            if (!lFinancial.Any())
                return null;

            var lOutput = new List<Stream>();

            lFinancial = lFinancial.OrderBy(x => x.d).ToList();
            var streamTonKho = await Chart_BanLe_TonKho(lFinancial, code);
            var streamNoTaiChinh = await Chart_BanLe_NoTaiChinh(lFinancial, code);
            var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), code);
            var streamThongKe = await Chart_ThongKe_BanLe();
            lOutput.Add(streamTonKho);
            lOutput.Add(streamNoTaiChinh);
            lOutput.Add(streamDoanhThu);
            lOutput.Add(streamThongKe);
            return lOutput;
        }

        private async Task<Stream> Chart_BanLe_TonKho(List<Financial> lFinancial, string code)
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
                _logger.LogError($"BllService.Chart_BanLe_TonKho|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_BanLe_NoTaiChinh(List<Financial> lFinancial, string code)
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
                _logger.LogError($"BllService.Chart_BanLe_NoTaiChinh|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_ThongKe_BanLe()
        {
            try
            {
                var lBanLe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.BanLe)).OrderBy(x => x.d);
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lBanLe.TakeLast(StaticVal._TAKE).Select(x => Math.Round(x.va/1000, 1)),
                        name = "Tổng mức bán lẻ",
                        type = "column",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lBanLe.TakeLast(StaticVal._TAKE).Select(x => x.qoq - 100),
                        name = "So với cùng kỳ",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                        yAxis = 1
                    }
                };

                return await Chart_BasicBase($"Tổng mức bán lẻ so với cùng kỳ năm ngoái(QoQ)", lBanLe.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameMonth()).ToList(), lSeries, "giá trị: nghìn tỷ", "giá trị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_ThongKe_BanLe|EXCEPTION| {ex.Message}");
            }

            return null;
        }
    }
}
