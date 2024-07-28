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
                input = input.ToUpper().Replace("LN_", "").Replace("CHART_", "").Trim();
                var exists = StaticVal.lHotKey.FirstOrDefault(x => x.Value.Any(y => input.Equals(y, StringComparison.CurrentCultureIgnoreCase)));
                if (exists.Key is null)
                    return null;

                var lStock = _stockRepo.GetByFilter(Builders<Stock>.Filter.ElemMatch(x => x.h24, y => Regex.IsMatch(y.code, exists.Key, RegexOptions.IgnoreCase)));
                if (!(lStock?.Any() ?? false))
                {
                    return null;
                }

                var lLN = new List<LN_Category>();
                var lStockClean = lStock.OrderByDescending(x => x.p.lv).Take(15);

                foreach (var item in lStockClean)
                {
                    var lFinance = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, item.s));
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
                        yearReport = cur.yearReport,
                        DoanhThu = (double)cur.revenue,
                        LoiNhuan = (double)cur.profit
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
                    //Ty Suat Loi Nhuan
                    model.TySuatLoiNhuan = model.DoanhThu == 0 ? int.MaxValue : Math.Round(model.LoiNhuan * 100 / model.DoanhThu, 1);

                    lLN.Add(model);
                }
                var maxD = lLN.Max(x => x.d);
                lLN = lLN.Where(x => x.d == maxD).ToList();

                var lDoanhThuOut = new List<double>();
                var lLoiNhuanOut = new List<double>();
                var lTySuatLN = new List<double>();
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
                        lDoanhThuOut.Add(0);
                        lLoiNhuanOut.Add(0);
                        lTySuatLN.Add(0);
                    }
                    else
                    {
                        lTangTruongDoanhThuOut.Add(first.TangTruongDoanhThu);
                        lTangTruongLoiNhuanOut.Add(first.TangTruongLoiNhuan);
                        lDoanhThuOut.Add(first.DoanhThu);
                        lLoiNhuanOut.Add(first.LoiNhuan);
                        lTySuatLN.Add(first.TySuatLoiNhuan);
                    }
                }
                

                var basicColumn = new HighchartBasicColumn($"Doanh Thu, Lợi Nhuận Quý {lLN.First().lengthReport}/{lLN.First().yearReport}", lCat, new List<HighChartSeries_BasicColumn>
                {
                     new HighChartSeries_BasicColumn
                    {
                        data = lDoanhThuOut,
                        name = "Doanh thu",
                        type = "column",
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLoiNhuanOut,
                        name = "Lợi nhuận",
                        type = "column",
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongDoanhThuOut,
                        name = "Tăng trưởng DT",
                        type = "spline",
                        color = "#012060",
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongLoiNhuanOut,
                        name = "Tăng trưởng LN",
                        type = "spline",
                        color = "#C00000",
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTySuatLN,
                        name = "Tỷ suất LN",
                        type = "spline",
                        color = "#ffbf00",
                        yAxis = 1
                    }
                });
                var strTitleYAxis = string.Empty;
                var lCheckValueUnit = basicColumn.series.Where(x => x.yAxis == 0).Select(x => x.data);
                foreach (var item in lCheckValueUnit)
                {
                    if(!item.Any(x => x != 0 && Math.Abs(x) < 1000000000))
                    {
                        strTitleYAxis = "(Đơn vị: tỷ)";
                        foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                        {
                            itemSeries.data = itemSeries.data.Select(x => x / 1000000000).ToList();
                        }
                        break;
                    }
                    if (!item.Any(x => x != 0 && Math.Abs(x) < 1000000))
                    {
                        strTitleYAxis = "(Đơn vị: triệu)";
                        foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                        {
                            itemSeries.data = itemSeries.data.Select(x => x / 1000000).ToList();
                        }
                        break;
                    }
                    if (!item.Any(x => x != 0 && Math.Abs(x) < 1000))
                    {
                        strTitleYAxis = "(Đơn vị: nghìn)";
                        foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                        {
                            itemSeries.data = itemSeries.data.Select(x => x / 1000).ToList();
                        }
                        break;
                    }
                }

                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

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
        public async Task<Stream> Chart_GG_Category(string input, string name, string sheetName)
        {
            try
            {
                input = input.ToUpper().Replace("LN_", "").Replace("CHART_", "").Trim();
                var exists = StaticVal.lHotKey.FirstOrDefault(x => x.Value.Any(y => input.Equals(y, StringComparison.CurrentCultureIgnoreCase)));
                if (exists.Key is null)
                    return null;
                var nhom = StaticVal.lHotKeyGG[exists.Key];

                var dt = DateTime.Now;
                var quarter = dt.GetQuarter();

                FilterDefinition<GoogleData> filter = null;
                var builder = Builders<GoogleData>.Filter;
                var lFilter = new List<FilterDefinition<GoogleData>>
                {
                    builder.Eq(x => x.nhom, nhom),
                    builder.Eq(x => x.sheet, sheetName)
                };
                foreach (var item in lFilter)
                {
                    if (filter is null)
                    {
                        filter = item;
                        continue;
                    }
                    filter &= item;
                }
                var lGG = _ggDataRepo.GetByFilter(filter);
                if(!(lGG?.Any()??false))
                {
                    return null;
                }
                //
                var maxYear = lGG.Max(x => x.year);
                var maxQuarter = lGG.Where(x => x.year == maxYear).Max(x => x.quarter);
                var quarterPrev = -1;
                var yearPrev = -1;
                if(maxQuarter > 1)
                {
                    quarterPrev = maxQuarter - 1;
                    yearPrev = maxYear;
                }
                else
                {
                    quarterPrev = 4;
                    yearPrev = maxYear - 1;
                }
                lGG = lGG.Where(x => (x.year == maxYear && x.quarter == maxQuarter)
                                    || (x.year == yearPrev && x.quarter == quarterPrev))
                        .OrderBy(x => x.code)
                        .ThenByDescending(x => x.year)
                        .ThenByDescending(x => x.quarter)
                        .ToList();

                var lCode = lGG.Select(x => x.code).Distinct().ToList();
                var lPrev = lGG.Where(x => x.year == yearPrev && x.quarter == quarterPrev && x.ty == 0).Select(x => new { s = x.code, val = (double)x.value }).ToList();
                var lCur = lGG.Where(x => x.year == maxYear && x.quarter == maxQuarter && x.ty == 0).Select(x => new { s = x.code, val = (double)x.value }).ToList();
                var lRate = lGG.Where(x => x.year == maxYear && x.quarter == maxQuarter && x.ty == 1).Select(x => new { s = x.code, val = (double)x.rate }).ToList();
                var lResult = new List<(string, double, double, double)>();
                foreach (var item in lCode)
                {
                    double valPrev = 0, valCur = 0, rateCur = 0;
                    var cur = lCur.FirstOrDefault(x => x.s == item);
                    if (cur != null)
                    {
                        valCur = cur.val;
                    }

                    var prev = lPrev.FirstOrDefault(x => x.s == item);
                    if(prev != null)
                    {
                        valPrev = prev.val;
                    }
                   
                    var rate = lRate.FirstOrDefault(x => x.s == item);
                    if(rate != null)
                    {
                        rateCur = rate.val;
                    }
                    lResult.Add((item, valCur, valPrev, rateCur));
                }

                var basicColumn = new HighchartBasicColumn($"{name} Quý {maxQuarter}/{maxYear}", lResult.Select(x => x.Item1).Distinct().ToList(), new List<HighChartSeries_BasicColumn>
                {
                     new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.Item2).ToList(),
                        name = $"{name} quý {maxQuarter}/{maxYear}",
                        type = "column",
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.Item3).ToList(),
                        name = $"{name} quý {quarterPrev}/{yearPrev}",
                        type = "column",
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lResult.Select(x => x.Item4).ToList(),
                        name = $"Tăng trưởng {name.ToLower()}",
                        type = "spline",
                        color = "#ffbf00",
                        yAxis = 1
                    }
                });
                var strTitleYAxis = string.Empty;
                var lCheckValueUnit = basicColumn.series.Where(x => x.yAxis == 0).Select(x => x.data);
                foreach (var item in lCheckValueUnit)
                {
                    if (!item.Any(x => x != 0 && Math.Abs(x) < 1000000000))
                    {
                        strTitleYAxis = "(Đơn vị: tỷ)";
                        foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                        {
                            itemSeries.data = itemSeries.data.Select(x => x / 1000000000).ToList();
                        }
                        break;
                    }
                    if (!item.Any(x => x != 0 && Math.Abs(x) < 1000000))
                    {
                        strTitleYAxis = "(Đơn vị: triệu)";
                        foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                        {
                            itemSeries.data = itemSeries.data.Select(x => x / 1000000).ToList();
                        }
                        break;
                    }
                    if (!item.Any(x => x != 0 && Math.Abs(x) < 1000))
                    {
                        strTitleYAxis = "(Đơn vị: nghìn)";
                        foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                        {
                            itemSeries.data = itemSeries.data.Select(x => x / 1000).ToList();
                        }
                        break;
                    }
                }

                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

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

        public async Task<Stream> Chart_ChienLuocDauTu()
        {
            try
            {
                var lData = new List<HighChartSeriesData_ChienLuocDauTu>
                {
                    new HighChartSeriesData_ChienLuocDauTu { id = "0", name = "2024", parent = string.Empty },
                    new HighChartSeriesData_ChienLuocDauTu { id = "1", name = "Tạo đáy", parent = "0" },
                    new HighChartSeriesData_ChienLuocDauTu { id = "2", name = "Phục hồi", parent = "0" },
                    new HighChartSeriesData_ChienLuocDauTu { id = "3", name = "Tăng trưởng", parent = "0" }
                };

                var lTangTruong = StaticVal.lTangTruong.ToList();
                lTangTruong.Reverse();
                foreach (var item in lTangTruong)
                {
                    lData.Add(new HighChartSeriesData_ChienLuocDauTu { id = "a", name = item, parent = "3" });
                }
                var lPhucHoi = StaticVal.lPhucHoi.ToList();
                lPhucHoi.Reverse();
                foreach (var item in lPhucHoi)
                {
                    lData.Add(new HighChartSeriesData_ChienLuocDauTu { id = "b", name = item, parent = "2" });
                }
                var lTaoDay = StaticVal.lTaoDay.ToList();
                lTaoDay.Reverse();
                foreach (var item in lTaoDay)
                {
                    lData.Add(new HighChartSeriesData_ChienLuocDauTu { id = "c", name = item, parent = "1" });
                }

                var series = new List<HighChartSeries_ChienLuocDauTu>
                {
                    new HighChartSeries_ChienLuocDauTu
                    {
                        type = "treegraph",
                        marker = new HighChartSeriesMarker_ChienLuocDauTu
                        {
                            symbol = "rect",
                            width = "25%"
                        },
                        borderRadius = 10,
                        levels = new List<HighChartSeriesLevel_ChienLuocDauTu>
                        {
                            new HighChartSeriesLevel_ChienLuocDauTu { level = 1 },
                            new HighChartSeriesLevel_ChienLuocDauTu { level = 2, colorByPoint = true },
                            new HighChartSeriesLevel_ChienLuocDauTu { level = 3, colorVariation = new HighChartSeriesColor_ChienLuocDauTu { key = "brightness", to = -0.5 } }
                        },
                        data = lData
                    }
                };
                var basicColumn = new HighChartChienLuocDauTu("Chiến lược đầu tư", series);
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

        public async Task<Stream> Chart_LN_Stock(string code)
        {
            try
            {
                var lFinance = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
                lFinance = lFinance?.Where(x => x.lengthReport > 0 && x.lengthReport < 5 && x.yearReport >= DateTime.Now.Year - 3).OrderByDescending(x => x.d).ToList();
                if (lFinance is null)
                    return null;
                //
                //tang truong doanh thu, tang truong loi nhuan
                var lLN = new List<LN_Category>();
                foreach (var item in lFinance)
                {
                    if (lLN.Any(x => x.d == item.d))
                        continue;

                    var cur = item;
                    var prev = lFinance.FirstOrDefault(x => x.yearReport == cur.yearReport - 1 && x.lengthReport == cur.lengthReport);
                    var model = new LN_Category
                    {
                        d = cur.d,
                        s = cur.s,
                        lengthReport = cur.lengthReport,
                        yearReport = cur.yearReport,
                        DoanhThu = (double)cur.revenue,
                        LoiNhuan = (double)cur.profit
                    };
                    if (prev is null)
                    {
                        model.TangTruongDoanhThu = 0;
                        model.TangTruongLoiNhuan = 0;
                    }
                    else
                    {
                        var rateRevenue = (cur.revenue / prev.revenue) ?? 0;
                        var rateProfit = (cur.profit / prev.profit) ?? 0;

                        model.TangTruongDoanhThu = (double)Math.Round((-1 + rateRevenue) * 100, 1);
                        model.TangTruongLoiNhuan = (double)Math.Round((-1 + rateProfit) * 100, 1); ;
                    }
                    //Ty Suat Loi Nhuan
                    model.TySuatLoiNhuan = model.DoanhThu == 0 ? int.MaxValue : Math.Round(model.LoiNhuan * 100 / model.DoanhThu, 1);

                    lLN.Add(model);
                }
                lLN.Reverse();

                var basicColumn = new HighchartBasicColumn($"Doanh Thu, Lợi Nhuận {code}", lLN.Select(x => $"{x.lengthReport}/{x.yearReport}").ToList(), new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lLN.Select(x => x.DoanhThu).ToList(),
                        name = "Doanh thu",
                        type = "column",
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLN.Select(x => x.LoiNhuan).ToList(),
                        name = "Lợi nhuận",
                        type = "column",
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLN.Select(x => x.TangTruongDoanhThu).ToList(),
                        name = "Tăng trưởng DT",
                        type = "spline",
                        color = "#012060",
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLN.Select(x => x.TangTruongLoiNhuan).ToList(),
                        name = "Tăng trưởng LN",
                        type = "spline",
                        color = "#C00000",
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLN.Select(x => x.TySuatLoiNhuan).ToList(),
                        name = "Tỷ suất LN",
                        type = "spline",
                        color = "#ffbf00",
                        yAxis = 1
                    }
                });
                var strTitleYAxis = "(Đơn vị: tỷ)";
                foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                {
                    itemSeries.data = itemSeries.data.Select(x => x / 1000000000).ToList();
                }

                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

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

        public async Task<Stream> Chart_GG_Stock(string code, string name, string sheetName, bool isShowIncrease = false)
        {
            try
            {
                FilterDefinition<GoogleData> filter = null;
                var builder = Builders<GoogleData>.Filter;
                var lFilter = new List<FilterDefinition<GoogleData>>
                {
                    builder.Eq(x => x.code, code),
                    builder.Eq(x => x.sheet, sheetName),
                    builder.Gte(x => x.year, DateTime.Now.Year - 3),
                    builder.Eq(x => x.ty, 0)
                };
                foreach (var item in lFilter)
                {
                    if (filter is null)
                    {
                        filter = item;
                        continue;
                    }
                    filter &= item;
                }
                var lGG = _ggDataRepo.GetByFilter(filter).OrderByDescending(x => x.year).ThenByDescending(x => x.quarter);
                if (!(lGG?.Any() ?? false))
                {
                    return null;
                }
                var lFinance = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
                lFinance = lFinance?.Where(x => x.lengthReport > 0 && x.lengthReport < 5 && x.yearReport >= DateTime.Now.Year - 3).OrderByDescending(x => x.d).ToList();
                //
                var lAnalyze = new List<(int, int, double, double, double)>();//year, quarter, val, rate, ton/doanh thu
                foreach (var item in lGG)
                {
                    if (lAnalyze.Any(x => x.Item1 == item.year && x.Item2 == item.quarter))
                        continue;

                    var cur = item;
                    (int, int, double, double, double) res;
                    res.Item1 = cur.year;
                    res.Item2 = cur.quarter;
                    res.Item3 = (double)cur.value;

                    int yearPrev = 0, quarterPrev = 0;
                    if(cur.quarter > 1)
                    {
                        yearPrev = cur.year;
                        quarterPrev = cur.quarter - 1;
                    }
                    else
                    {
                        yearPrev = cur.year - 1;
                        quarterPrev = 4;
                    }

                    var prev = lGG.FirstOrDefault(x => x.year == yearPrev && x.quarter == quarterPrev);
                    if(prev is null)
                    {
                        res.Item4 = 0;
                    }
                    else
                    {
                        res.Item4 = (double)Math.Round(100 * (-1 + cur.value / prev.value), 1);
                    }

                    var financial = lFinance.FirstOrDefault(x => x.yearReport == cur.year && x.lengthReport == cur.quarter);
                    if (financial is null)
                    {
                        res.Item5 = 0;
                    }
                    else
                    {
                        res.Item5 = (double)Math.Round(100 * (cur.value - (prev?.value ?? cur.value)) / (financial.revenue ?? int.MaxValue), 1);
                    }
                    lAnalyze.Add(res);
                }
                lAnalyze.Reverse();

                var series = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lAnalyze.Select(x => x.Item3).ToList(),
                        name = $"Giá trị",
                        type = "column",
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lAnalyze.Select(x => x.Item4).ToList(),
                        name = $"Tăng trưởng QoQ",
                        type = "spline",
                        color = "#C00000",
                        yAxis = 1
                    }
                };

                if(isShowIncrease)
                {
                    series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lAnalyze.Select(x => x.Item5).ToList(),
                        name = $"Gia tăng {name.ToLower()}/ Doanh thu",
                        type = "spline",
                        color = "#ffbf00",
                        yAxis = 1
                    });
                }

                var basicColumn = new HighchartBasicColumn($"{name} {code}", lAnalyze.Select(x => $"{x.Item2}/{x.Item1}").ToList(), series);
                var strTitleYAxis = "(Đơn vị: tỷ)";
                foreach (var itemSeries in basicColumn.series.Where(x => x.yAxis == 0))
                {
                    itemSeries.data = itemSeries.data.Select(x => x / 1000000000).ToList();
                }

                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

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

        public async Task<Stream> Chart_KeHoachNam_Stock(string code, List<KeHoachThucHienAPIData> lData)
        {
            try
            {
                if (lData is null)
                    return null;

                lData = lData.Where(x => x.year >= (DateTime.Now.Year - 4)).ToList();
                var lAnalyze = new List<(int, double, double, double, double)>();//year, doanh thu ke hoach, loi nhuan ke hoach, rate doanh thu, rate loi nhuan
                foreach (var item in lData)
                {
                    if (lAnalyze.Any(x => x.Item1 == item.year))
                        continue;

                    var cur = item;
                    (int, double, double, double, double) res;
                    res.Item1 = cur.year;
                    res.Item2 = (double)cur.isa3;
                    res.Item3 = (double)cur.isa22;

                    var real = item.quarter.FirstOrDefault(x => x.quarter == 0 || x.quarter == 5);
                    if(real is null)
                    {
                        res.Item4 = 0;
                        res.Item5 = 0;
                    }
                    else
                    {
                        res.Item4 = (double)real.isa3_percent;
                        res.Item5 = (double)real.isa22_percent;
                    }
                    lAnalyze.Add(res);
                }
                lAnalyze.Reverse();

                var series = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lAnalyze.Select(x => x.Item2).ToList(),
                        name = $"Doanh thu KH",
                        type = "column",
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lAnalyze.Select(x => x.Item3).ToList(),
                        name = $"LNST KH",
                        type = "column",
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lAnalyze.Select(x => x.Item4).ToList(),
                        name = $"Doanh thu thực hiện",
                        type = "spline",
                        color = "#012060",
                        yAxis = 1
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lAnalyze.Select(x => x.Item5).ToList(),
                        name = $"LNST thực hiện",
                        type = "spline",
                        color = "#C00000",
                        yAxis = 1
                    }
                };

                var basicColumn = new HighchartBasicColumn($"Kế hoạch năm {code}", lAnalyze.Select(x => $"{x.Item1}").ToList(), series);
                var strTitleYAxis = "(Đơn vị: tỷ)";
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

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
            public double DoanhThu { get; set; }
            public double LoiNhuan { get; set; }
            public double TangTruongDoanhThu { get; set; }
            public double TangTruongLoiNhuan { get; set; }
            public double TySuatLoiNhuan { get; set; }
            public int yearReport { get; set; }
            public int lengthReport { get; set; }
        }
    }
}
