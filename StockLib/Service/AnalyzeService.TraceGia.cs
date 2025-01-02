﻿using Microsoft.Extensions.Logging;
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
                res += $"\n       => {model.description}\n";
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

                var lTraceGia = new List<TraceGiaModel>();
                var strOutput = new StringBuilder();

                #region Giá Thịt Lợn
                var pig = await _apiService.Pig333_GetPigPrice();
                var modelPig = new TraceGiaModel
                {
                    content = "Giá thịt heo",
                    description = "DBC,BAF,HAG"
                };
                var isPrintPig = false;
                if (pig != null && pig.Any())
                {
                    try
                    {
                        var last = pig.Last();
                        //weekly
                        var dtPrev = dt.AddDays(-2);
                        if (last.Date >= dtPrev)
                        {
                            var lastWeek = pig.SkipLast(1).Last();
                            var rateWeek = Math.Round(100 * (-1 + last.Value / lastWeek.Value), 1);
                            modelPig.weekly = rateWeek;
                            if (rateWeek >= flag || rateWeek <= -flag)
                            {
                                isPrintPig = true;
                            }
                        }
                        //Monthly
                        var dtMonthly = dt.AddMonths(-1);
                        var itemMonthly = pig.Where(x => x.Date <= dtMonthly).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (itemMonthly != null)
                        {
                            var rateMonthly = Math.Round(100 * (-1 + last.Value / itemMonthly.Value), 1);
                            modelPig.monthly = rateMonthly;
                        }
                        //yearly
                        var dtYearly = dt.AddYears(-1);
                        var itemYearly = pig.Where(x => x.Date <= dtYearly).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (itemYearly != null)
                        {
                            var rateYearly = Math.Round(100 * (-1 + last.Value / itemYearly.Value), 1);
                            modelPig.yearly = rateYearly;
                        }
                        //YTD
                        var dtYTD = new DateTime(dt.Year, 1, 2);
                        var itemYTD = pig.Where(x => x.Date <= dtYTD).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (itemYTD != null)
                        {
                            var rateYTD = Math.Round(100 * (-1 + last.Value / itemYTD.Value), 1);
                            modelPig.YTD = rateYTD;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                //Print
                if (isAll || isPrintPig)
                {
                    lTraceGia.Add(modelPig);
                } 
                #endregion

                #region WCI Index
                var wci = await _apiService.MacroMicro_WCI("44756");
                var modelWCI = new TraceGiaModel
                {
                    content = "Cước Container",
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
                            if (lData.Any())
                            {
                                var last = lData.Last();
                                //weekly
                                var dtPrev = dt.AddDays(-2);
                                if (last.Date >= dtPrev)
                                {
                                    var lastWeek = lData.SkipLast(1).Last();
                                    var rateWeek = Math.Round(100 * (-1 + last.Value / lastWeek.Value), 1);
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
                                    var rateMonthly = Math.Round(100 * (-1 + last.Value / itemMonthly.Value), 1);
                                    modelWCI.monthly = rateMonthly;
                                }
                                //yearly
                                var dtYearly = dt.AddYears(-1);
                                var itemYearly = lData.Where(x => x.Date <= dtYearly).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemYearly != null)
                                {
                                    var rateYearly = Math.Round(100 * (-1 + last.Value / itemYearly.Value), 1);
                                    modelWCI.yearly = rateYearly;
                                }
                                //YTD
                                var dtYTD = new DateTime(dt.Year, 1, 2);
                                var itemYTD = lData.Where(x => x.Date <= dtYTD).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemYTD != null)
                                {
                                    var rateYTD = Math.Round(100 * (-1 + last.Value / itemYTD.Value), 1);
                                    modelWCI.YTD = rateYTD;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                //Print
                if (isAll || isPrintWCI)
                {
                    lTraceGia.Add(modelWCI);
                }
                #endregion
                #region Yellow Photphorus Index
                var lPhotpho = await _apiService.Metal_GetYellowPhotpho();
                var modelPhotpho = new TraceGiaModel
                {
                    content = "Phốt pho vàng",
                    description = "DGC,PAT"
                };
                var isPrintPhotpho = false;
                if (lPhotpho?.Any() ?? false)
                {
                    try
                    {
                        foreach (var item in lPhotpho)
                        {
                            item.metalsPrice.Date = item.metalsPrice.renewDate.ToDateTime("yyyy-MM-dd");
                        }
                        var cur = lPhotpho.First();
                        //weekly
                        var dtPrev = dt.AddDays(-2);
                        if (cur.metalsPrice.Date >= dtPrev)
                        {
                            var nearTime = cur.metalsPrice.Date.AddDays(-6);
                            var itemWeekly = lPhotpho.Where(x => x.metalsPrice.Date <= nearTime).OrderByDescending(x => x.metalsPrice.Date).First();
                            var rateWeek = Math.Round(100 * (-1 + cur.metalsPrice.average / itemWeekly.metalsPrice.average), 1);
                            modelPhotpho.weekly = rateWeek;
                            if (rateWeek >= flag || rateWeek <= -flag)
                            {
                                isPrintPhotpho = true;
                            }
                        }
                        //Monthly
                        var dtMonthly = dt.AddMonths(-1);
                        var itemMonthly = lPhotpho.Where(x => x.metalsPrice.Date <= dtMonthly).OrderByDescending(x => x.metalsPrice.Date).FirstOrDefault();
                        if (itemMonthly != null)
                        {
                            var rateMonthly = Math.Round(100 * (-1 + cur.metalsPrice.average / itemMonthly.metalsPrice.average), 1);
                            modelPhotpho.monthly = rateMonthly;
                        }
                        //yearly
                        var dtYearly = dt.AddYears(-1);
                        var itemYearly = lPhotpho.Where(x => x.metalsPrice.Date <= dtYearly).OrderByDescending(x => x.metalsPrice.Date).FirstOrDefault();
                        if (itemYearly != null)
                        {
                            var rateYearly = Math.Round(100 * (-1 + cur.metalsPrice.average / itemYearly.metalsPrice.average), 1);
                            modelPhotpho.yearly = rateYearly;
                        }
                        //YTD
                        var dtYTD = new DateTime(dt.Year, 1, 2);
                        var itemYTD = lPhotpho.Where(x => x.metalsPrice.Date <= dtYTD).OrderByDescending(x => x.metalsPrice.Date).FirstOrDefault();
                        if (itemYTD != null)
                        {
                            var rateYTD = Math.Round(100 * (-1 + cur.metalsPrice.average / itemYTD.metalsPrice.average), 1);
                            modelPhotpho.YTD = rateYTD;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                //Print
                if (isAll || isPrintPhotpho)
                {
                    lTraceGia.Add(modelPhotpho);
                }
                #endregion

                #region BDTI Index
                var bdti = await _apiService.MacroMicro_WCI("946");
                var modelBDTI = new TraceGiaModel
                {
                    content = "Cước vận tải dầu thô",
                    description = "PVT,VTO"
                };
                var isPrintBDTI = false;
                if (bdti != null)
                {

                    var composite = bdti.series.FirstOrDefault();
                    if (composite != null)
                    {
                        try
                        {
                            var lData = composite.Select(x => new MacroMicro_CleanData
                            {
                                Date = x[0].ToDateTime("yyyy-MM-dd"),
                                Value = decimal.Parse(x[1])
                            });
                            if (lData.Any())
                            {
                                var last = lData.Last();
                                //weekly
                                var dtPrev = dt.AddDays(-2);
                                if (last.Date >= dtPrev)
                                {
                                    var lastWeek = lData.SkipLast(1).Last();
                                    var rateWeek = Math.Round(100 * (-1 + last.Value / lastWeek.Value), 1);
                                    modelBDTI.weekly = rateWeek;
                                    if (rateWeek >= flag || rateWeek <= -flag)
                                    {
                                        isPrintBDTI = true;
                                    }
                                }
                                //Monthly
                                var dtMonthly = dt.AddMonths(-1);
                                var itemMonthly = lData.Where(x => x.Date <= dtMonthly).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemMonthly != null)
                                {
                                    var rateMonthly = Math.Round(100 * (-1 + last.Value / itemMonthly.Value), 1);
                                    modelBDTI.monthly = rateMonthly;
                                }
                                //yearly
                                var dtYearly = dt.AddYears(-1);
                                var itemYearly = lData.Where(x => x.Date <= dtYearly).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemYearly != null)
                                {
                                    var rateYearly = Math.Round(100 * (-1 + last.Value / itemYearly.Value), 1);
                                    modelBDTI.yearly = rateYearly;
                                }
                                //YTD
                                var dtYTD = new DateTime(dt.Year, 1, 2);
                                var itemYTD = lData.Where(x => x.Date <= dtYTD).OrderByDescending(x => x.Date).FirstOrDefault();
                                if (itemYTD != null)
                                {
                                    var rateYTD = Math.Round(100 * (-1 + last.Value / itemYTD.Value), 1);
                                    modelBDTI.YTD = rateYTD;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                //Print
                if (isAll || isPrintBDTI)
                {
                    lTraceGia.Add(modelBDTI);
                }
                #endregion

                var lEconomic = await _apiService.Tradingeconimic_Commodities();
                foreach (var item in lEconomic)
                {
                    if (isAll || item.Weekly >= flag || item.Weekly <= -flag) 
                    {
                        if(item.Code == EPrice.Crude_Oil.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Dầu thô",
                                description = "PLX,OIL",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if(item.Code == EPrice.Natural_gas.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Khí tự nhiên",
                                description = "GAS,DCM,DPM",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.kraftpulp.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Bột giấy",
                                description = "DHC",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Coal.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Than",
                                description = "",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Gold.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Vàng",
                                description = "",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Steel.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Thép",
                                description = "HPG",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.HRC_Steel.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "HRC",
                                description = "HSG,NKG,GDA",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Rubber.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Cao su",
                                description = "TRC,DRI",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Coffee.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Cà phê",
                                description = "",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Rice.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Gạo",
                                description = "LTG",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Sugar.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Đường",
                                description = "SLS,LSS,SBT,QNS",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Urea.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "U rê",
                                description = "DPM,DCM",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.polyvinyl.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Hạt nhựa PVC",
                                description = "BMP,NTP",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.Nickel.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Niken",
                                description = "PC1",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                        else if (item.Code == EPrice.milk.GetDisplayName())
                        {
                            lTraceGia.Add(new TraceGiaModel
                            {
                                content = "Sữa",
                                description = "VNM",
                                weekly = item.Weekly,
                                monthly = item.Monthly,
                                yearly = item.YoY,
                                YTD = item.YTD
                            });
                        }
                    }
                }

                foreach (var item in lTraceGia.OrderByDescending(x => x.weekly).ThenBy(x => x.monthly))
                {
                    strOutput.AppendLine(PrintTraceGia(item));
                }

                if(isAll)
                {
                    _configRepo.InsertOne(new ConfigData
                    {
                        ty = (int)mode,
                        t = t
                    });
                }

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
