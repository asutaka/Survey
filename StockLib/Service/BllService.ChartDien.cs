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
               
                var streamNo = await Chart_Dien_NoTaiChinh(lInput, lFinancial);
                lOutput.Add(streamNo);

                var streamThongKe = await Chart_ThongKe_Dien();
                lOutput.Add(streamThongKe);

                var streamThongKeQuy = await Chart_ThongKeQuy_Dien();
                lOutput.Add(streamThongKeQuy);
                
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

        private async Task<Stream> Chart_ThongKe_Dien()
        {
            try
            {
                var lBanLe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.IIP_Dien)).OrderBy(x => x.d);
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
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

                return await Chart_BasicBase($"Sản xuất và phân phối điện tháng so với cùng kỳ năm ngoái(MoM)", lBanLe.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameMonth()).ToList(), lSeries, "Đơn vị: %", "Đơn vị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_ThongKe_Dien|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        private async Task<Stream> Chart_ThongKeQuy_Dien()
        {
            try
            {
                var lBanLe = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.GiaNVL_Dien)).OrderBy(x => x.d);
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
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

                return await Chart_BasicBase($"Giá điện quý so với cùng kỳ năm ngoái(QoQ)", lBanLe.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameQuarter()).ToList(), lSeries, "Đơn vị: %", "Đơn vị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_ThongKeQuy_Dien|EXCEPTION| {ex.Message}");
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
            var streamNoTaiChinh = await Chart_Dien_NoTaiChinh(lFinancial, code);
            var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), code);
            var streamThongKe = await Chart_ThongKe_Dien();
            var streamThongKeQuy = await Chart_ThongKeQuy_Dien();

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
