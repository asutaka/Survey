using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Survey.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Survey.Utils
{
    public class Data
    {
        /// <summary>
        /// Lấy danh sách toàn bộ coin của Binnance và sắp xếp theo tỉ lệ tăng giá giảm dần
        /// </summary>
        /// <returns></returns>
        /// 
        private static IEnumerable<TicketModel> _lCoin = null;
        public static IEnumerable<TicketModel> GetCoins(int top = 0)
        {
            if (_lCoin != null)
                return _lCoin;

            var settings = 0.LoadJsonFile<AppsettingModel>("appsettings"); 
            var content = WebClass.GetWebContent(settings.API.API24hr).GetAwaiter().GetResult();
            if (!string.IsNullOrWhiteSpace(content))
            {
                _lCoin = JsonConvert.DeserializeObject<IEnumerable<TicketModel>>(content)
                            .Where(x => x.symbol.EndsWith("USDT"))
                            .OrderByDescending(x => x.priceChangePercent)
                            .ToList();

                if (top > 0)
                    _lCoin = _lCoin.Take(top);
            }
            return _lCoin;
        }

        /// <summary>
        /// Lấy ra 500 bản ghi theo interval và symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static IEnumerable<FinancialDataPoint> GetData(string symbol, EInterval interval)
        {
            var lData = new List<FinancialDataPoint>();
            
            try
            {
                var settings = 0.LoadJsonFile<AppsettingModel>("appsettings");
                var url = (interval == EInterval.I15M) ? settings.API.History15M : settings.API.History1H;
                var content = WebClass.GetWebContent(string.Format(url, symbol.ToUpper())).GetAwaiter().GetResult();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var arr = JArray.Parse(content);
                    lData.AddRange(arr.Select(x =>
                    new FinancialDataPoint((((long)x[0]) / 1000).UnixTimeStampToDateTime(),
                                            double.Parse(x[1].ToString()),
                                            double.Parse(x[2].ToString()),
                                            double.Parse(x[3].ToString()),
                                            double.Parse(x[4].ToString()),
                                            double.Parse(x[5].ToString())))
                                                .ToList());
                }
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"Data.GetData|EXCEPTION| {ex.Message}");
            }
            return lData;
        }
    }
}
