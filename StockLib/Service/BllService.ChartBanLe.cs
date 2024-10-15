using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;

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
    }
}
