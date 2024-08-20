using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Skender.Stock.Indicators;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public interface IAnalyzeService
    {
        Task<(int, string)> ChiBaoMA20();
        Task<(int, string)> ChiBao52W();
        Task<(int, string)> ThongkeNhomNganh(DateTime dt);
        Task<(int, string)> ThongkeForeign(DateTime dt);
        Task<(int, string)> ThongKeTuDoanhHNX(DateTime dt);
        Task<(int, string)> ThongKeTuDoanhUp(DateTime dt);
        Task<(int, string)> ThongKeTuDoanhHSX(DateTime dt);
    }
    public class AnalyzeService : IAnalyzeService
    {
        private readonly ILogger<AnalyzeService> _logger;
        private readonly IAPIService _apiService;
        private readonly IFileService _fileService;
        private readonly IStockRepo _stockRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly ITuDoanhRepo _tudoanhRepo;
        public AnalyzeService(ILogger<AnalyzeService> logger,
                            IAPIService apiService,
                            IFileService fileService,
                            IStockRepo stockRepo,
                            IConfigDataRepo configRepo,
                            ICategoryRepo categoryRepo,
                            ITuDoanhRepo tudoanhRepo) 
        {
            _logger = logger;
            _apiService = apiService;
            _fileService = fileService;
            _stockRepo = stockRepo;
            _configRepo = configRepo;
            _categoryRepo = categoryRepo;
            _tudoanhRepo = tudoanhRepo;
        }
        public async Task<(int, string)> ChiBaoMA20()
        {
            try
            {
                var strOutput = new StringBuilder();

                var lMa = await _apiService.Money24h_GetMaTheoChiBao_MA20();
                if (lMa is null
                    || !lMa.Any())
                    return (0, null);

                var lMaClean = lMa.Where(x => x.match_price > x.basic_price && x.accumylated_vol > 5000).OrderByDescending(x => Math.Abs(x.change_vol_percent_5));
                var lStock = _stockRepo.GetAll();
                var lStockClean = new List<Stock>();
                foreach (var item in lMaClean)
                {
                    var entityStock = lStock.FirstOrDefault(x => x.s == item.symbol && x.status > 0);
                    if (entityStock != null)
                        lStockClean.Add(entityStock);
                }

                if (!lStockClean.Any())
                    return (0, null);

                var lOut = new List<MaTheoPTKT24H_MA20>();
                foreach (var item in lStockClean)
                {
                    var lData = await _apiService.SSI_GetDataStock(item.s);
                    if (lData.Count() < 250
                        || lData.Last().Volume < 50000)
                        continue;

                    var lMa20 = lData.GetSma(20);
                    var lIchi = lData.GetIchimoku();

                    //Analyze
                    var entity = lData.Last();
                    var ma20 = lMa20.Last();
                    var entityNear = lData.SkipLast(1).TakeLast(1).First();
                    var ma20Near = lMa20.SkipLast(1).TakeLast(1).First();

                    if (entityNear.Open < (decimal)ma20Near.Sma
                        && entityNear.Close < (decimal)ma20Near.Sma
                        && entity.Close >= (decimal)ma20.Sma)
                    {
                        var model = new MaTheoPTKT24H_MA20
                        {
                            s = item.s,
                            rank = item.rank
                        };
                        var ichi = lIchi.Last();
                        if (entity.Close >= ichi.SenkouSpanA
                            && entity.Close >= ichi.SenkouSpanB)
                        {
                            model.isIchi = true;
                        }

                        foreach (var itemVolume in lData)
                        {
                            itemVolume.Close = itemVolume.Volume;
                        }
                        var lMa20Volume = lData.GetSma(20);
                        var vol = lMa20Volume.Last();

                        if (entity.Volume > (decimal)vol.Sma
                            && entity.Volume > entityNear.Volume)
                        {
                            model.isVol = true;
                        }

                        lOut.Add(model);
                    }
                }

                if (!lOut.Any())
                    return (0, null);

                strOutput.AppendLine($"[Thông báo] Top cổ phiếu vừa cắt lên MA20:");
                var index = 1;
                foreach (var item in lOut.OrderBy(x => x.rank).Take(10).ToList())
                {
                    var content = $"{index}. {item.s}";
                    var extend = string.Empty;
                    if (item.isIchi)
                    {
                        extend += "ichimoku";
                    }
                    if (item.isVol)
                    {
                        extend = (string.IsNullOrWhiteSpace(extend)) ? "vol đột biến" : $"{extend}, vol đột biến";
                    }
                    if (!string.IsNullOrWhiteSpace(extend))
                    {
                        content += $"- {extend}";
                    }
                    strOutput.AppendLine(content);
                    index++;
                }

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.ChiBaoMA20|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

        public async Task<(int, string)> ChiBao52W()
        {
            try
            {
                var strOutput = new StringBuilder();

                var lMa = await _apiService.Money24h_GetMaTheoChiBao_52W();
                if (lMa is null
                    || !lMa.Any())
                    return (0, null);

                var lMaClean = lMa.Where(x => x.match_price > x.basic_price && x.accumylated_vol > 5000).OrderByDescending(x => Math.Abs(x.change_vol_percent_5));
                var lStock = _stockRepo.GetAll();
                var lStockClean = new List<Stock>();
                foreach (var item in lMaClean)
                {
                    var entityStock = lStock.FirstOrDefault(x => x.s == item.symbol && x.status > 0);
                    if (entityStock != null)
                        lStockClean.Add(entityStock);
                }

                if (!lStockClean.Any())
                    return (0, null);

                strOutput.AppendLine($"[Thông báo] Top cổ phiếu vừa vượt đỉnh 52 Week:");
                var index = 1;
                foreach (var item in lStockClean.OrderBy(x => x.rank).Take(10).ToList())
                {
                    var content = $"{index}. {item.s}";
                    var extend = string.Empty;
                    if (!string.IsNullOrWhiteSpace(extend))
                    {
                        content += $"- {extend}";
                    }
                    strOutput.AppendLine(content);
                    index++;
                }

                return (1, strOutput.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.ChiBao52W|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }

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
                lData.AddRange(await _apiService.Money24h_GetForeign(EMoney24hExchangeMode.HSX, type));
                lData.AddRange(await _apiService.Money24h_GetForeign(EMoney24hExchangeMode.HNX, type));
                lData.AddRange(await _apiService.Money24h_GetForeign(EMoney24hExchangeMode.UPCOM, type));
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
                if(lTopBuy.Any())
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

                    var content = $"{index++}. {item.s} (Mua ròng {item.net} tỷ)";
                    if (item.net_pt != 0)
                    {
                        var buySell_pt = item.net_pt > 0 ? "Thỏa thuận mua" : "Thỏa thuận bán";
                        content += $" - {buySell_pt} {Math.Abs(item.net_pt)} tỷ";
                    }
                    strOutput.AppendLine(content);
                }
                if (lTopSell.Any())
                {
                    strOutput.AppendLine($"*Top bán ròng:");
                }
                index = 1;
                foreach (var item in lTopSell)
                {
                    item.net = Math.Round(item.net / 1000000, 1);
                    item.net_pt = Math.Round(item.net_pt / 1000000, 1);
                    if (item.net == 0)
                        continue;

                    var content = $"{index++}. {item.s} (Bán ròng {item.net} tỷ)";
                    if (item.net_pt != 0)
                    {
                        var buySell_pt = item.net_pt > 0 ? "Thỏa thuận mua" : "Thỏa thuận bán";
                        content += $" - {buySell_pt} {Math.Abs(item.net_pt)} tỷ";
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
                //Check Exists
                FilterDefinition<TuDoanh> filter = null;
                var builder = Builders<TuDoanh>.Filter;
                var lFilter = new List<FilterDefinition<TuDoanh>>();
                if (string.IsNullOrWhiteSpace(item.s))
                    continue;

                lFilter.Add(builder.Eq(x => x.s, item.s));
                lFilter.Add(builder.Eq(x => x.d, item.d));
                foreach (var itemFilter in lFilter)
                {
                    if (filter is null)
                    {
                        filter = itemFilter;
                        continue;
                    }
                    filter &= itemFilter;
                }

                if (filter is null)
                    return null;


                var lFind = _tudoanhRepo.GetByFilter(filter, 1, 20);
                if ((lFind ?? new List<TuDoanh>()).Any())
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
                _tudoanhRepo.InsertOne(model);
            }
            return lstResult;
        }

        private class MaTheoPTKT24H_MA20
        {
            public string s { get; set; }
            public int rank { get; set; }
            public bool isIchi { get; set; }
            public bool isVol { get; set; }
        }
    }
}
