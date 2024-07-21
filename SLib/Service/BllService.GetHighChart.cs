using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SLib.Model;
using SLib.Model.APIModel;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<Stream> Chart_VonHoa_Category(string input)
        {
            try
            {
                input = input.ToUpper().Replace("CHART_VONHOA_", "").Trim();
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
    }
}
