using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<Stream> Chart_Dien_DoanhThu_LoiNhuan(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();
                var lFinancial = _dienRepo.GetByFilter(Builders<Financial_Dien>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _dienRepo.GetByFilter(Builders<Financial_Dien>.Filter.Eq(x => x.d, int.Parse($"{time.Item2 - 1}{time.Item3}")));
                return await Chart_DoanhThuBase(lMaCK, (int)time.Item1, lFinancial.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), null, isTangTruongLoiNhuan: true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Dien_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_Dien_NoTrenVonChu(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();

                var lFinancial = _dienRepo.GetByFilter(Builders<Financial_Dien>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var lVonChu = new List<double>();
                var lNo = new List<double>();
                var lNoTrenVonChu = new List<double>();
                foreach (var item in lMaCK)
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

                    //t
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
                        name = "Nợ tài chính",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lNoTrenVonChu,
                        name = "Nợ trên vốn chủ sở hữu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Nợ trên vốn chủ sở hữu Quý {time.Item3}/{time.Item2}", lMaCK, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Dien_NoTrenVonChu|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
