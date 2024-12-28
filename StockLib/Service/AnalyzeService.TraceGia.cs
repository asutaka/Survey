using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        private string PrintTraceGia(TraceGiaModel model)
        {
            var res = $"   - {model.content}: W({model.weekly}%)|M({model.monthly}%)|Y({model.yearly}%)|YTD({model.YTD}%)";
            if (!string.IsNullOrWhiteSpace(model.description))
            {
                res += $"|=> {model.description}";
            }
            return res;
        }
        public async Task<(int, string)> TraceGia(DateTime dt, bool isAll)
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

                var wci = await _apiService.MacroMicro_WCI();
                var modelWCI = new TraceGiaModel
                {
                    content = "Giá cước Container",
                    description = "HAH"
                };
                var isPrintWCI = false;
                if (wci != null)
                {
                    
                    var composite = wci.series.FirstOrDefault();
                    if (composite != null)
                    {
                        try
                        {
                            var lData = composite.Select(x => new MacroMicro_CleanData
                            {
                                Date = x[0].ToDateTime("yyyy-MM-dd"),
                                Value = decimal.Parse(x[1])
                            });
                            if(lData.Any())
                            {
                                var last = lData.Last();
                                //weekly
                                var dtPrev = dt.AddDays(-2);
                                if (last.Date >= dtPrev)
                                {
                                    var lastWeek = lData.SkipLast(1).Last();
                                    var rateWeek = Math.Round(100 * (-1 + last.Value / lastWeek.Value));
                                    modelWCI.weekly = rateWeek;
                                    if (rateWeek >= flag || rateWeek <= -flag)
                                    {
                                        isPrintWCI = true;
                                    }
                                }
                                //Monthly
                                var dtMonthly = dt.AddMonths(-1);
                                var itemMonthly = lData.Where(x => x.Date <= dtMonthly).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemMonthly != null)
                                {
                                    var rateMonthly = Math.Round(100 * (-1 + last.Value / itemMonthly.Value));
                                    modelWCI.monthly = rateMonthly;
                                }
                                //yearly
                                var dtYearly = dt.AddYears(-1);
                                var itemYearly = lData.Where(x => x.Date <= dtYearly).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemYearly != null)
                                {
                                    var rateYearly = Math.Round(100 * (-1 + last.Value / itemYearly.Value));
                                    modelWCI.yearly = rateYearly;
                                }
                                //YTD
                                var dtYTD = new DateTime(dt.Year, 1, 2);
                                var itemYTD = lData.Where(x => x.Date <= dtYTD).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemYTD != null)
                                {
                                    var rateYTD = Math.Round(100 * (-1 + last.Value / itemYTD.Value));
                                    modelWCI.YTD = rateYTD;
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                //Print
                if (isAll || isPrintWCI)
                {
                    strOutput.AppendLine(PrintTraceGia(modelWCI));
                }
                //photpho vang
                var lPhotpho = await _apiService.Metal_GetYellowPhotpho();
                if (lPhotpho?.Any() ?? false)
                {
                    var cur = lPhotpho.First();
                    var time = cur.metalsPrice.renewDate.ToDateTime("yyyy-MM-dd");
                    var nearTime = time.AddDays(-6);

                    var near = lPhotpho.FirstOrDefault(x => x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{nearTime.Day.To2Digit()}"
                                                        || x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{(nearTime.Day - 1).To2Digit()}"
                                                        || x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{(nearTime.Day - 2).To2Digit()}"
                                                        || x.metalsPrice.renewDate == $"{nearTime.Year}-{nearTime.Month.To2Digit()}-{(nearTime.Day - 3).To2Digit()}");
                    if (near != null)
                    {
                        var prev = lPhotpho.FirstOrDefault(x => x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{time.Day.To2Digit()}");
                        if (prev is null)
                        {
                            prev = lPhotpho.FirstOrDefault(x => x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{time.Day.To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 1).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 2).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 3).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 4).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 5).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day + 6).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 1).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 2).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 3).To2Digit()}"
                                                            || x.metalsPrice.renewDate == $"{time.AddYears(-1).Year}-{time.Month.To2Digit()}-{(time.Day - 4).To2Digit()}");
                        }

                        var strMes = string.Empty;
                        var rate = Math.Round(100 * (-1 + cur.metalsPrice.average / near.metalsPrice.average), 1);
                        if (isAll)
                        {
                            strMes = $"- Giá phốt pho vàng(weekly): {rate}%";
                            if (prev != null)
                            {
                                var ratePrev = Math.Round(100 * (-1 + cur.metalsPrice.average / prev.metalsPrice.average), 1);
                                strMes += $" |YoY: {Math.Round(ratePrev, 1)}% | => DGC, PAT";
                            }
                        }
                        else
                        {
                            if (rate >= flag || rate <= -flag)
                            {
                                strMes = $"- Giá phốt pho vàng(weekly): {rate}%";
                                if (prev != null)
                                {
                                    var ratePrev = Math.Round(100 * (-1 + cur.metalsPrice.average / prev.metalsPrice.average), 1);
                                    strMes += $" |YoY: {Math.Round(ratePrev, 1)}% |=> DGC, PAT";
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(strMes))
                        {
                            strOutput.AppendLine(strMes);
                        }
                    }
                }
                
                //var wci = await _apiService.Drewry_WCI();
                //if(isAll)
                //{
                //    strOutput.AppendLine($"   - Giá cước Container(weekly): {wci.Item1}%| YoY: {wci.Item2}% |=> HAH");
                //}
                //else
                //{
                //    if (wci.Item1 >= flag || wci.Item1 <= -flag)
                //    {
                //        strOutput.AppendLine($"   - Giá cước Container(weekly): {wci.Item1}%| YoY: {wci.Item2}%|=> HAH");
                //    }
                //}

                var bdti = await _apiService.Macrovar_Commodities();
                if (bdti.ow >= flag || bdti.ow <= -flag)
                {
                    strOutput.AppendLine($"   - Cước vận tải dầu thô(weekly): {bdti.ow}%| YoY: {bdti.oy}");
                }

                var lEconomic = await _apiService.Tradingeconimic_Commodities();
                foreach (var item in lEconomic)
                {
                    if (item.Weekly >= flag || item.Weekly <= -flag) 
                    {
                        if(item.Code == EPrice.Crude_Oil.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Dầu Thô(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if(item.Code == EPrice.Natural_gas.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Khí Tự Nhiên(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Coal.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Than(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Gold.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Vàng(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Steel.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Thép(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.HRC_Steel.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Thép HRC(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Rubber.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Cao Su(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Coffee.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Cà Phê(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Rice.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Gạo(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Sugar.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Đường(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Urea.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá U rê(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.polyvinyl.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá nhựa PVC(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.Nickel.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Niken-PC1(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
                        }
                        else if (item.Code == EPrice.milk.GetDisplayName())
                        {
                            strOutput.AppendLine($"   - Giá Sữa(weekly): {Math.Round(item.Weekly, 1)}% |Monthly: {Math.Round(item.Monthly, 1)}% |YTD: {Math.Round(item.YTD, 1)}% |YoY: {Math.Round(item.YoY, 1)}%");
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
