using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_Dien(IEnumerable<string> lInput)
        {
            try
            {
                var lOutput = new List<Stream>();
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();
                var lFinancial = _dienRepo.GetByFilter(Builders<Financial_Dien>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;
                var streamThongKe = await Chart_Dien_ThongKeThang();
                lOutput.Add(streamThongKe);

                var streamThongKeQuy = await Chart_Dien_ThongKeQuy();
                lOutput.Add(streamThongKeQuy);

                var streamNo = await Chart_Dien_NoTaiChinh(lInput, lFinancial);
                lOutput.Add(streamNo);
                return lOutput;

            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Dien|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_Dien_NoTaiChinh(IEnumerable<string> lInput, List<Financial_Dien> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();
                var lOrderBy = new List<Financial_Dien>();
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
                _logger.LogError($"BllService.Chart_Dien_NoTaiChinh|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_Dien_ThongKeThang()
        {
            try
            {
                var time = GetCurrentTime();
                var filter = Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP);
                var lThongKe = _thongkeRepo.GetByFilter(filter);
                lThongKe = lThongKe.Where(x => x.content.RemoveSpace().RemoveSignVietnamese().Contains("Phan Phoi Dien".RemoveSpace(), StringComparison.OrdinalIgnoreCase))
                                    .OrderBy(x => x.d).TakeLast(StaticVal._TAKE).ToList();

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe.Select(x => Math.Round(x.qoq - 100, 1)).ToList(),
                        name = "Phân phối điện(so với cùng kỳ)",
                        type = "line",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe.Select(x => Math.Round(x.qoqoy - 100, 1)).ToList(),
                        name = "Phân phối điện(so với tháng trước)",
                        type = "line",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#ffbf00"
                    }
                };
                return await Chart_BasicBase($"Thống kê phân phối điện theo tháng", lThongKe.Select(x => x.d.GetNameMonth()).ToList(), lSeries, titleX: "%");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Dien_ThongKeThang|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_Dien_ThongKeQuy()
        {
            try
            {
                var time = GetCurrentTime();
                var filter = Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.GiaNVL_Dien);
                var lThongKe = _thongkequyRepo.GetByFilter(filter);
                lThongKe = lThongKe.OrderBy(x => x.d).TakeLast(StaticVal._TAKE).ToList();

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe.Select(x => Math.Round(x.qoq - 100, 1)).ToList(),
                        name = "Giá điện(so với cùng kỳ)",
                        type = "line",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe.Select(x => Math.Round(x.qoqoy - 100, 1)).ToList(),
                        name = "Giá điện(so với tháng trước)",
                        type = "line",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#ffbf00"
                    }
                };
                return await Chart_BasicBase($"Thống kê giá điện theo quý", lThongKe.Select(x => x.d.GetNameQuarter()).ToList(), lSeries, titleX: "%");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Dien_ThongKeQuy|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<List<Stream>> Chart_Dien(string code)
        {
            var lFinancial = _dienRepo.GetByFilter(Builders<Financial_Dien>.Filter.Eq(x => x.s, code));
            if (!lFinancial.Any())
                return null;

            var lOutput = new List<Stream>();

            lFinancial = lFinancial.OrderBy(x => x.d).ToList();
            var streamThongKe = await Chart_Dien_ThongKeThang();
            var streamThongKeQuy = await Chart_Dien_ThongKeQuy();
            var streamNoTaiChinh = await Chart_Dien_NoTaiChinh(lFinancial, code);
            var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), code);
            
            lOutput.Add(streamThongKe);
            lOutput.Add(streamThongKeQuy);
            lOutput.Add(streamNoTaiChinh);
            lOutput.Add(streamDoanhThu);
            return lOutput;
        }

        private async Task<Stream> Chart_Dien_NoTaiChinh(List<Financial_Dien> lFinancial, string code)
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
                _logger.LogError($"BllService.Chart_Dien_NoTaiChinh|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
