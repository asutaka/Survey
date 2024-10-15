using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_Logistic(IEnumerable<string> lInput)
        {
            try
            {
                var lOutput = new List<Stream>();
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();
                var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var streamNo = await Chart_Logistic_NoTaiChinh(lInput, lFinancial);
                lOutput.Add(streamNo);

                var streamThongKe = await Chart_ThongKeQuy_Logistic();
                lOutput.Add(streamThongKe);
                return lOutput;

            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Logistic|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_Logistic_NoTaiChinh(IEnumerable<string> lInput, List<Financial> lFinancial)
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
                _logger.LogError($"BllService.Chart_Logistic_NoTaiChinh|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<List<Stream>> Chart_Logistic(string code)
        {
            var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
            if (!lFinancial.Any())
                return null;

            var lOutput = new List<Stream>();

            lFinancial = lFinancial.OrderBy(x => x.d).ToList();
            var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), code);
            lOutput.Add(streamDoanhThu);

            var streamNoTaiChinh = await Chart_NoTaiChinh(lFinancial, code);
            lOutput.Add(streamNoTaiChinh);

            var streamThongKe = await Chart_ThongKeQuy_Logistic();
            lOutput.Add(streamThongKe);
            
            return lOutput;
        }

        private async Task<Stream> Chart_ThongKeQuy_Logistic()
        {
            try
            {
                var lThongKe_GiaVTBien = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.QUY_GiaVT_Bien)).OrderBy(x => x.d);
                var lThongKe_GiaVTBuuChinh = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.QUY_GiaVT_BuuChinh)).OrderBy(x => x.d);
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe_GiaVTBien.TakeLast(StaticVal._TAKE).Select(x => x.qoq - 100),
                        name = "Giá VT Biển",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe_GiaVTBuuChinh.TakeLast(StaticVal._TAKE).Select(x => x.qoq - 100),
                        name = "Giá vận tải bưu chính",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                };

                return await Chart_BasicBase($"Giá vận tải quý so với cùng kỳ năm ngoái(QoQ)", lThongKe_GiaVTBien.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameQuarter()).ToList(), lSeries, "Đơn vị: %", "Đơn vị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_ThongKeQuy_Logistic|EXCEPTION| {ex.Message}");
            }

            return null;
        }
    }
}
