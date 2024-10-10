using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> TinHieuMuaBanCoin()
        {
            try
            {
                var lCoin = _coinRepo.GetAll();

                var lSuperTrend = new List<Coin>();
                var lSuperTrendPhrase2 = new List<Coin>();

                foreach (var item in lCoin)
                {
                    if (item is null
                        || !item.indicator.Any())
                        continue;

                    var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, item.s, Bybit.Net.Enums.KlineInterval.FourHours, null, null, 1000);
                    var lData = lByBit.Data.List.Select(x => new Quote
                    {
                        Date = x.StartTime,
                        Open = x.OpenPrice,
                        High = x.HighPrice,
                        Low = x.LowPrice,
                        Close = x.ClosePrice,
                        Volume = x.Volume,
                    }).ToList();

                    if (item.indicator.Any(x => x.ty == (int)EIndicator.SuperTrend))
                    {
                        var isSuperTrend = lData.CheckSuperTrend();
                        if (isSuperTrend)
                        {
                            lSuperTrend.Add(item);
                        }
                    }


                    if (item.indicator.Any(x => x.ty == (int)EIndicator.SuperTrendPhrase2))
                    {
                        var isSuperTrendPhrase2 = lData.CheckSuperTrendPharse2();
                        if (isSuperTrendPhrase2)
                        {
                            lSuperTrendPhrase2.Add(item);
                        }
                    }
                }

                var sBuilder = new StringBuilder();

                if (lSuperTrend.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu SuperTrend - COIN]");

                    foreach (var item in lSuperTrend.Take(10))
                    {
                        var indicator = item.indicator.FirstOrDefault(x => x.ty == (int)EIndicator.SuperTrend);
                        sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg}%| Win/Loss: {indicator.win}%/{indicator.loss}%)");
                    }
                }

                if (lSuperTrendPhrase2.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu SuperTrend - Phrase 2 - COIN]");

                    foreach (var item in lSuperTrendPhrase2.Take(10))
                    {
                        var indicator = item.indicator.FirstOrDefault(x => x.ty == (int)EIndicator.SuperTrendPhrase2);
                        sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg}%| Win/Loss: {indicator.win}%/{indicator.loss}%)");
                    }
                }

                if (string.IsNullOrWhiteSpace(sBuilder.ToString()))
                {
                    return (0, null);
                }
                return (1, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TinHieuMuaBanCoin|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }
    }
}
