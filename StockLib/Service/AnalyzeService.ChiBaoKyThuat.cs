using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.DAL.Entity;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
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

        private class MaTheoPTKT24H_MA20
        {
            public string s { get; set; }
            public int rank { get; set; }
            public bool isIchi { get; set; }
            public bool isVol { get; set; }
        }
    }
}
