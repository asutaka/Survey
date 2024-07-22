using MongoDB.Driver;
using Newtonsoft.Json;
using SLib.Model;
using SLib.Model.APIModel;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<Stream> Chart_VonHoa_Category(string input)
        {
            try
            {
                input = input.ToUpper().Replace("VONHOA_", "").Trim();
                var exists = StaticVal.lHotKey.FirstOrDefault(x => x.Value.Any(y => input.Equals(y, StringComparison.CurrentCultureIgnoreCase)));
                if (exists.Key is null)
                    return null;
               
                var filter = Builders<Stock>.Filter.ElemMatch(x => x.h24, y => Regex.IsMatch(y.code, exists.Key, RegexOptions.IgnoreCase));
                var lStock = _stockRepo.GetByFilter(filter);
                if (!(lStock?.Any() ?? false))
                {
                    return null;
                }

                var lSeries = new List<List<object>>();
                foreach (var item in lStock.OrderByDescending(x => x.p.lv).Take(20))
                {
                    lSeries.Add(new List<object>
                    {
                        item.s,
                        Math.Round(item.p.lv/1000000000)
                    });
                }

                var basicColumn = new HighChartBasicColumnCustomColor(exists.Value.First(), "Vốn hóa" , lSeries);
                var chart = new HighChartAPIModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<Stream> Chart_LN_Category(string input)
        {
            try
            {
                input = input.ToUpper().Replace("LN_", "").Trim();
                var exists = StaticVal.lHotKey.FirstOrDefault(x => x.Value.Any(y => input.Equals(y, StringComparison.CurrentCultureIgnoreCase)));
                if (exists.Key is null)
                    return null;

                var filter = Builders<Stock>.Filter.ElemMatch(x => x.h24, y => Regex.IsMatch(y.code, exists.Key, RegexOptions.IgnoreCase));
                var lStock = _stockRepo.GetByFilter(filter);
                if (!(lStock?.Any() ?? false))
                {
                    return null;
                }

                var lLN = new List<LN_Category>();
                var lStockClean = lStock.OrderByDescending(x => x.p.lv).Take(15);

                foreach (var item in lStockClean)
                {
                    var filterFinance = Builders<Financial>.Filter.Eq(x => x.s, item.s);
                    var lFinance = _financialRepo.GetByFilter(filterFinance);
                    lFinance = lFinance?.Where(x => x.lengthReport > 0 && x.lengthReport < 5).OrderByDescending(x => x.d).ToList();
                    if (lFinance is null)
                        continue;
                    //
                    //tang truong doanh thu, tang truong loi nhuan
                    var cur = lFinance.First();
                    var prev = lFinance.FirstOrDefault(x => x.yearReport == cur.yearReport - 1 && x.lengthReport == cur.lengthReport);
                    var model = new LN_Category
                    {
                        d = cur.d,
                        s = cur.s,
                        lengthReport = cur.lengthReport,
                        yearReport = cur.yearReport
                    };
                    if (prev is null)
                    {
                        model.TangTruongDoanhThu = 0;
                        model.TangTruongLoiNhuan = 0;
                    }
                    else
                    {
                        var rateRevenue = (cur.revenue / prev.revenue)?? 0;
                        var rateProfit = (cur.profit / prev.profit) ?? 0;

                        model.TangTruongDoanhThu = (double)Math.Round((-1 + rateRevenue) * 100, 1);
                        model.TangTruongLoiNhuan = (double)Math.Round((-1 + rateProfit) * 100, 1); ;
                    }

                    lLN.Add(model);
                }
                var maxD = lLN.Max(x => x.d);
                lLN = lLN.Where(x => x.d == maxD).ToList();

                var lTangTruongDoanhThuOut = new List<double>();
                var lTangTruongLoiNhuanOut = new List<double>();
                var lCat = lStockClean.Select(x => x.s).ToList();
                foreach (var item in lCat)
                {
                    var first = lLN.FirstOrDefault(x => x.s == item);
                    if (first is null)
                    {
                        lTangTruongDoanhThuOut.Add(0);
                        lTangTruongLoiNhuanOut.Add(0);
                    }
                    else
                    {
                        lTangTruongDoanhThuOut.Add(first.TangTruongDoanhThu);
                        lTangTruongLoiNhuanOut.Add(first.TangTruongLoiNhuan);
                    }
                }

                var basicColumn = new HighchartBasicColumn($"Tăng trưởng doanh thu, lợi nhuận quý {lLN.First().lengthReport}/{lLN.First().yearReport}", lCat, new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongDoanhThuOut,
                        name = "Doanh thu",
                        type = "column"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongLoiNhuanOut,
                        name = "Lợi nhuận",
                        type = "column"
                    }
                });
                var chart = new HighChartAPIModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<Stream> GetBasicColumn()
        {
            var basicColumn = new HighchartBasicColumn("test", new List<string> { "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec"}, new List<HighChartSeries_BasicColumn>
            {
                new HighChartSeries_BasicColumn
                {
                    data = new List<double> { 29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4 },
                    name = string.Empty,
                }
            });
            var chart = new HighChartAPIModel(JsonConvert.SerializeObject(basicColumn));
            var body = JsonConvert.SerializeObject(chart);
            return await _apiService.GetChartImage(body);
        }

        public async Task<Stream> GetBasicColumnCustomColor()
        {
            var basicColumn = new HighChartBasicColumnCustomColor("test", "ngân hàng", new List<List<object>> { 
                new List<object>{ "Tokyo", 37.33 },
                new List<object>{ "Delhi", 31.18 },
                new List<object>{ "Shanghai", 27.79 },
                new List<object>{ "Sao Paulo", 22.23 },
                new List<object>{ "Mexico City", 21.91 },
                new List<object>{ "Dhaka", 21.74 },
                new List<object>{ "Cairo", 21.32 },
                new List<object>{ "Beijing", 20.89 },
                new List<object>{ "Mumbai", 20.67 },
                new List<object>{ "Osaka", 19.11 },
                new List<object>{ "Karachi", 16.45 },
                new List<object>{ "Chongqing", 16.38 },
                new List<object>{ "Istanbul", 15.41 },
                new List<object>{ "Buenos Aires", 15.25 },
                new List<object>{ "Kolkata", 14.974 },
                new List<object>{ "Kinshasa", 14.970 },
                new List<object>{ "Lagos", 14.86 },
                new List<object>{ "Manila", 14.16 },
                new List<object>{ "Tianjin", 13.79 },
                new List<object>{ "Guangzhou", 13.64 }
            });
            var chart = new HighChartAPIModel(JsonConvert.SerializeObject(basicColumn));
            var body = JsonConvert.SerializeObject(chart);
            return await _apiService.GetChartImage(body);
        }
        private class LN_Category
        {
            public long d { get; set; }
            public string s { get; set; }
            public double TangTruongDoanhThu { get; set; }
            public double TangTruongLoiNhuan { get; set; }
            public int yearReport { get; set; }
            public int lengthReport { get; set; }
        }
    }
}
