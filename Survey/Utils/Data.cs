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
        public static IEnumerable<TicketModel> GetCoins(int top = 0)
        {
            IEnumerable<TicketModel> _lCoin = null;
            var settings = 0.LoadJsonFile<AppsettingModel>("appsettings"); 
            var content = HelperUtils.GetContent(settings.API.API24hr);
            if (!string.IsNullOrWhiteSpace(content))
            {
                _lCoin = JsonConvert.DeserializeObject<IEnumerable<TicketModel>>(content)
                            .Where(x => x.symbol.EndsWith("USDT")
                                    && !x.symbol.EndsWith("UPUSDT")
                                    && !x.symbol.EndsWith("DOWNUSDT"))
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
                var url = string.Empty;
                if (interval == EInterval.I15M)
                    url = settings.API.History15M;
                else if (interval == EInterval.I1H)
                    url = settings.API.History1H;
                else if (interval == EInterval.I4H)
                    url = settings.API.History4H;
                else if (interval == EInterval.I1D)
                    url = settings.API.History1D;
                
                //var content = WebClass.GetWebContent(string.Format(url, symbol.ToUpper())).GetAwaiter().GetResult();
                var content = HelperUtils.GetJsonArray(string.Format(url, symbol.ToUpper()));
                lData.AddRange(content.Select(x =>
                    new FinancialDataPoint((((long)x[0]) / 1000).UnixTimeStampToDateTime(),
                                            double.Parse(x[1].ToString()),
                                            double.Parse(x[2].ToString()),
                                            double.Parse(x[3].ToString()),
                                            double.Parse(x[4].ToString()),
                                            double.Parse(x[5].ToString())))
                                                .ToList());
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"Data.GetData|EXCEPTION| {ex.Message}");
            }
            return lData;
        }

        public static IEnumerable<FinancialDataPoint> GetDataAll(string symbol, EInterval interval, int max = 0)
        {
            var lData = new List<FinancialDataPoint>();

            try
            {
                var settings = 0.LoadJsonFile<AppsettingModel>("appsettings");
                var url = string.Empty;
                if (interval == EInterval.I15M)
                    url = settings.API.History15MTime;
                else if (interval == EInterval.I1H)
                    url = settings.API.History1HTime;
                else if (interval == EInterval.I4H)
                    url = settings.API.History4HTime;
                else if (interval == EInterval.I1D)
                    url = settings.API.History1DTime;
                if (string.IsNullOrWhiteSpace(url))
                    return lData;

                var contentM = HelperUtils.GetJsonArray(string.Format(settings.API.History1M, symbol.ToUpper()));
                if (contentM != null && contentM.Any())
                {
                    var time = ((long)contentM[0][0]) / 1000;
                    JArray content = null;
                    do
                    {
                        content = HelperUtils.GetJsonArray(string.Format(url, symbol.ToUpper(), time));
                        if (content != null && content.Any())
                        {
                            int countArr = content.Count;
                            lData.AddRange(content.Select(x =>
                                            new FinancialDataPoint((((long)x[0]) / 1000).UnixTimeStampToDateTime(),
                                                                    double.Parse(x[1].ToString()),
                                                                    double.Parse(x[2].ToString()),
                                                                    double.Parse(x[3].ToString()),
                                                                    double.Parse(x[4].ToString()),
                                                                    double.Parse(x[5].ToString())))
                                                                        .ToList());
                            time = (countArr > 0) ? (long)content[countArr-1][0] : 0;
                            if (max > 0 && lData.Count() >= max)
                                break;
                        }
                    }
                    while (content != null && content.Count() >= 500);
                }
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"Data.GetDataAll|EXCEPTION| {ex.Message}");
            }
            return lData;
        }
    }
}
