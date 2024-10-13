using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> TraceGia(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            var dTime = new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day)).ToUnixTimeSeconds();
            var flag = 7;
            try
            {
                var mode = EConfigDataType.TraceGia;
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)mode);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var wci = await _apiService.Drewry_WCI();
                if (wci.Item1 >= flag || wci.Item1 <= -flag)
                {
                    strOutput.AppendLine($"   - Giá cước Container(weekly): {wci.Item1}%| YoY: {wci.Item2}%");
                }

                var lBDTI = await _apiService.MacroVar_GetData("84286"); //BDTI: cước vận tải dầu
                if (lBDTI?.Any() ?? false) 
                {
                    var lTake = lBDTI.TakeLast(7);
                    var last = lTake.Last();
                    var timeLast = last.date.ToDateTime("yyyy-MM-dd");

                    if (timeLast.Day == dt.Day
                        && timeLast.Month == dt.Month
                        && timeLast.Year == dt.Year)
                    {
                        foreach (var item in lTake.Reverse())
                        {
                            var time = item.date.ToDateTime("yyyy-MM-dd");
                            if ((timeLast - time).TotalDays >= 6)
                            {
                                var rate = Math.Round(100 * (-1 + last.value / item.value), 1);
                                if (rate >= flag || rate <= -flag)
                                {
                                    strOutput.AppendLine($"   - Giá cước vận tải dầu thô(weekly): {rate}%");
                                    break;
                                }
                            }
                        }
                    }
                }

                var lEconomic = await _apiService.Tradingeconimic_Commodities();
                foreach (var item in lEconomic)
                {
                    if (item.Weekly >= flag || item.Weekly <= -flag) 
                    {
                        if(item.Code == "Crude Oil")
                        {
                            strOutput.AppendLine($"   - Giá Dầu Thô(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if(item.Code == "Natural gas")
                        {
                            strOutput.AppendLine($"   - Giá Khí Tự Nhiên(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Coal")
                        {
                            strOutput.AppendLine($"   - Giá Than(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Gold")
                        {
                            strOutput.AppendLine($"   - Giá Vàng(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Steel")
                        {
                            strOutput.AppendLine($"   - Giá Thép(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "HRC Steel")
                        {
                            strOutput.AppendLine($"   - Giá Thép HRC(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Rubber")
                        {
                            strOutput.AppendLine($"   - Giá Cao Su(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Coffee")
                        {
                            strOutput.AppendLine($"   - Giá Cà Phê(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Rice")
                        {
                            strOutput.AppendLine($"   - Giá Gạo(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Sugar")
                        {
                            strOutput.AppendLine($"   - Giá Đường(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "Urea")
                        {
                            strOutput.AppendLine($"   - Giá U rê(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == "polyvinyl")
                        {
                            strOutput.AppendLine($"   - Giá nhựa PVC(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                    }
                }

                //_configRepo.InsertOne(new ConfigData
                //{
                //    ty = (int)mode,
                //    t = t
                //});
                if (strOutput.Length > 0)
                {
                    strOutput.Insert(0,"[Giá một số ngành hàng]\n");
                    return (1, strOutput.ToString());
                }   
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TraceGia|EXCEPTION| {ex.Message}");
            }
            return (0, string.Empty);
        }
    }
}
