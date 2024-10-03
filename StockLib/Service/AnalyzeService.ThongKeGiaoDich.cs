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
        public async Task<(int, string)> ThongkeNhomNganh(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            var dTime = new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day)).ToUnixTimeSeconds();
            try
            {
                var type = EMoney24hTimeType.today;
                var mode = EConfigDataType.ThongKeNhomNganh_today;
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

                var res = await _apiService.Money24h_GetNhomNganh(type);
                if (res is null
                    || !res.data.groups.Any()
                    || res.data.last_update < dTime)
                    return (0, null);

                var lData = _categoryRepo.GetAll();
                var lNhomNganhData = new List<Money24h_NhomNganh_GroupResponse>();
                foreach (var itemLv1 in res.data.groups)
                {
                    var findLv1 = lData.FirstOrDefault(x => x.code == itemLv1.icb_code);
                    if (findLv1 != null)
                    {
                        itemLv1.icb_name = findLv1.name;
                        lNhomNganhData.Add(itemLv1);
                        continue;
                    }

                    foreach (var itemLv2 in itemLv1.child)
                    {
                        var findLv2 = lData.FirstOrDefault(x => x.code == itemLv2.icb_code);
                        if (findLv2 != null)
                        {
                            itemLv2.icb_name = findLv2.name;
                            lNhomNganhData.Add(itemLv2);
                            continue;
                        }

                        foreach (var itemLv3 in itemLv2.child)
                        {
                            var findLv3 = lData.FirstOrDefault(x => x.code == itemLv3.icb_code);
                            if (findLv3 != null)
                            {
                                itemLv3.icb_name = findLv3.name;
                                lNhomNganhData.Add(itemLv3);
                                continue;
                            }

                            foreach (var itemLv4 in itemLv3.child)
                            {
                                var findLv4 = lData.FirstOrDefault(x => x.code == itemLv4.icb_code);
                                if (findLv4 != null)
                                {
                                    itemLv4.icb_name = findLv4.name;
                                    lNhomNganhData.Add(itemLv4);
                                }
                            }
                        }
                    }
                }

                if (!lNhomNganhData.Any())
                    return (0, null);

                lNhomNganhData = lNhomNganhData.OrderByDescending(x => (float)x.total_stock_increase / x.total_stock).Take(5).ToList();


                var head = $"[Thông báo] Nhóm ngành được quan tâm ngày {dt.ToString("dd/MM/yyyy")}:";
                strOutput.AppendLine(head);
                var index = 1;
                foreach (var item in lNhomNganhData)
                {
                    var content = $"{index}. {item.icb_name}({Math.Round((float)item.total_stock_increase * 100 / item.total_stock, 1)}%)";
                    strOutput.AppendLine(content);
                    index++;
                }

                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)mode,
                    t = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.ThongkeNhomNganh|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        public async Task<(int, string)> ThongkeForeign(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            var dTime = new DateTimeOffset(new DateTime(dt.Year, dt.Month, dt.Day)).ToUnixTimeSeconds();
            try
            {
                var type = EMoney24hTimeType.today;
                var mode = EConfigDataType.GDNN_today;
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
                var lData = new List<Money24h_ForeignResponse>();
                lData.AddRange(await _apiService.Money24h_GetForeign(EExchange.HSX, type));
                lData.AddRange(await _apiService.Money24h_GetForeign(EExchange.HNX, type));
                lData.AddRange(await _apiService.Money24h_GetForeign(EExchange.UPCOM, type));
                if (!lData.Any())
                    return (0, null);

                var head = $"[Thông báo] GDNN ngày {dt.ToString("dd/MM/yyyy")}:"; ;
                strOutput.AppendLine(head);
                strOutput.AppendLine($"*Top mua ròng:");
                var lTopBuy = lData.OrderByDescending(x => x.net_val).Take(10);
                var lTopSell = lData.OrderBy(x => x.net_val).Take(10);
                var index = 1;
                foreach (var item in lTopBuy)
                {
                    var content = $"{index}. {item.s} (Mua ròng {Math.Abs(item.net_val).ToString("#,##0.00")} tỷ)";
                    strOutput.AppendLine(content);
                    index++;
                }

                strOutput.AppendLine();
                strOutput.AppendLine($"*Top bán ròng:");
                index = 1;
                foreach (var item in lTopSell)
                {
                    var content = $"{index}. {item.s} (Bán ròng {Math.Abs(item.net_val).ToString("#,##0.00")} tỷ)";
                    strOutput.AppendLine(content);
                    index++;
                }

                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.GDNN_today,
                    t = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.ThongkeForeign|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        public async Task<(int, string)> ThongKeTuDoanhHNX(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TuDoanhHNX);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, string.Empty);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.TuDoanhHNX(EHnxExchange.NY, dt);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, string.Empty);

                var lData = _fileService.HNX(stream);
                var lOutput = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh HNX ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Chi tiết:");
                lOutput = lOutput.OrderByDescending(x => x.net).ToList();
                var index = 1;
                foreach (var item in lOutput)
                {
                    var buySell = item.net > 0 ? "Mua ròng" : "Bán ròng";
                    var valStr = string.Empty;
                    item.net = Math.Abs(item.net / 1000);
                    if (item.net < 1000)
                    {
                        valStr = $"{Math.Round(item.net, 0)} triệu";
                    }
                    else
                    {
                        item.net = Math.Round(item.net / 1000, 1);
                        valStr = $"{item.net} tỷ";
                    }
                    strOutput.AppendLine($"{index++}. {item.s} ({buySell} {valStr})");
                }
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.TuDoanhHNX,
                    t = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.ThongKeTuDoanhHNX|EXCEPTION| {ex.Message}");
            }

            return (0, string.Empty);
        }

        public async Task<(int, string)> ThongKeTuDoanhUp(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TuDoanhUpcom);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, string.Empty);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.TuDoanhHNX(EHnxExchange.UP, dt);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, string.Empty);

                var lData = _fileService.HNX(stream);
                var lOutput = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh Upcom ngày {dt.ToString("dd/MM/yyyy")}:");
                strOutput.AppendLine($"*Chi tiết:");
                lOutput = lOutput.OrderByDescending(x => x.net).ToList();
                var index = 1;
                foreach (var item in lOutput)
                {
                    var buySell = item.net > 0 ? "Mua ròng" : "Bán ròng";
                    var valStr = string.Empty;
                    item.net = Math.Abs(item.net / 1000);
                    if (item.net < 1000)
                    {
                        valStr = $"{Math.Round(item.net, 0)} triệu";
                    }
                    else
                    {
                        item.net = Math.Round(item.net / 1000, 1);
                        valStr = $"{item.net} tỷ";
                    }
                    strOutput.AppendLine($"{index++}. {item.s} ({buySell} {valStr})");
                }
                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.TuDoanhUpcom,
                    t = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.ThongKeTuDoanhUp|EXCEPTION| {ex.Message}");
            }

            return (0, string.Empty);
        }

        public async Task<(int, string)> ThongKeTuDoanhHSX(DateTime dt)
        {
            var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
            try
            {
                var builder = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filter = builder.Eq(x => x.ty, (int)EConfigDataType.TuDoanhHose);
                var lConfig = _configRepo.GetByFilter(filter);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filter);
                }

                var strOutput = new StringBuilder();
                var stream = await _apiService.TuDoanhHSX(dt);
                if (stream is null
                    || stream.Length < 1000)
                    return (0, null);

                var lData = _fileService.HSX(stream);
                var lOutput = InsertTuDoanh(lData);

                strOutput.AppendLine($"[Thông báo] Tự doanh HOSE ngày {dt.ToString("dd/MM/yyyy")}:");
                var lTopBuy = lOutput.Where(x => x.net > 0).OrderByDescending(x => x.net).Take(10);
                var lTopSell = lOutput.Where(x => x.net < 0).OrderBy(x => x.net).Take(10);
                if (lTopBuy.Any())
                {
                    strOutput.AppendLine($"*Top mua ròng:");
                }
                var index = 1;
                foreach (var item in lTopBuy)
                {
                    item.net = Math.Round(item.net / 1000000, 1);
                    item.net_pt = Math.Round(item.net_pt / 1000000, 1);
                    if (item.net == 0)
                        continue;

                    var content = $"{index++}. {item.s} (Mua ròng {Math.Abs(item.net).ToString("#,##0.#")} tỷ)";
                    if (item.net_pt != 0)
                    {
                        var buySell_pt = item.net_pt > 0 ? "Thỏa thuận mua" : "Thỏa thuận bán";
                        content += $" - {buySell_pt} {Math.Abs(item.net_pt).ToString("#,##0.#")} tỷ";
                    }
                    strOutput.AppendLine(content);
                }
                if (lTopSell.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top bán ròng:");
                }
                index = 1;
                foreach (var item in lTopSell)
                {
                    item.net = Math.Round(item.net / 1000000, 1);
                    item.net_pt = Math.Round(item.net_pt / 1000000, 1);
                    if (item.net == 0)
                        continue;

                    var content = $"{index++}. {item.s} (Bán ròng {Math.Abs(item.net).ToString("#,##0.#")} tỷ)";
                    if (item.net_pt != 0)
                    {
                        var buySell_pt = item.net_pt > 0 ? "Thỏa thuận mua" : "Thỏa thuận bán";
                        content += $" - {buySell_pt} {Math.Abs(item.net_pt).ToString("#,##0.#")} tỷ";
                    }
                    strOutput.AppendLine(content);
                }

                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.TuDoanhHose,
                    t = t
                });

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.ThongKeTuDoanhHSX|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        private List<TuDoanh> InsertTuDoanh(List<TudoanhPDF> lInput)
        {
            var lstResult = new List<TuDoanh>();
            foreach (var item in lInput)
            {
                if (string.IsNullOrWhiteSpace(item.s))
                    continue;

                var model = new TuDoanh
                {
                    no = item.no,
                    d = item.d,
                    s = item.s,
                    net_deal = item.bva - item.sva,
                    net_pt = item.bva_pt - item.sva_pt,
                    t = item.t
                };
                model.net = model.net_deal + model.net_pt;
                lstResult.Add(model);
            }
            return lstResult;
        }
    }
}
