using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<Stream> Chart_Thep_DoanhThu_LoiNhuan(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();
                var lFinancial = _thepRepo.GetByFilter(Builders<Financial_Thep>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var lFinancialPrev = _thepRepo.GetByFilter(Builders<Financial_Thep>.Filter.Eq(x => x.d, int.Parse($"{time.Item2 - 1}{time.Item3}")));
                return await Chart_DoanhThuBase(lMaCK, (int)time.Item1, lFinancial.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), lFinancialPrev?.Select(x => (x.s, x.rv, x.pf, x.pfg, x.pfn)), null, isBienLoiNhuanGop: true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Thep_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_Thep_TonKho(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();
                var lFinancial = _thepRepo.GetByFilter(Builders<Financial_Thep>.Filter.Eq(x => x.d, time.Item1));
                if (!lFinancial.Any())
                    return null;

                var yearPrev = time.Item2;
                var quarterPrev = time.Item3;
                if (time.Item3 > 1)
                {
                    quarterPrev--;
                }
                else
                {
                    quarterPrev = 4;
                    yearPrev--;
                }
                var lFinancialPrev = _thepRepo.GetByFilter(Builders<Financial_Thep>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lTonKho = new List<double>();
                var lTangTruongTonKho = new List<double>();
                foreach (var item in lMaCK)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lTonKho.Add(0);
                        lTangTruongTonKho.Add(0);
                        continue;
                    }

                    double tangTruong = 0;
                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev is not null && prev.inv > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
                    }

                    //tang truong tin dung, room tin dung
                    lTonKho.Add(cur.inv);
                    lTangTruongTonKho.Add(tangTruong);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTonKho,
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongTonKho,
                        name = "Tăng trưởng tồn kho",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"Tồn kho Quý {time.Item3}/{time.Item2} (QoQ)", lMaCK, lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_Thep_TonKho|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> Chart_Thep_NoTrenVonChu(IEnumerable<string> lInput)
        {
            try
            {
                var lMaCK = lInput.Take(15).ToList();
                var time = GetCurrentTime();

                var lFinancial = _thepRepo.GetByFilter(Builders<Financial_Thep>.Filter.Eq(x => x.d, time.Item1));
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
                _logger.LogError($"BllService.Chart_Thep_NoTrenVonChu|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
