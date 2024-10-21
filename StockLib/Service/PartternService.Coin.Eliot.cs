using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyCoinEliot(string code)
        {
            try
            {
                _code = code;
                var lByBit = await StaticVal.ByBitInstance().V5Api.ExchangeData.GetKlinesAsync(Bybit.Net.Enums.Category.Spot, code, Bybit.Net.Enums.KlineInterval.OneDay, null, null, 1000);
                var lData = lByBit.Data.List.Select(x => new Quote
                {
                    Date = x.StartTime,
                    Open = x.OpenPrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Close = x.ClosePrice,
                    Volume = x.Volume,
                }).ToList();

                lData.Reverse();

                await SurveyCoinEliot(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyEliot|EXCEPTION| {ex.Message}");
            }
        }

        private async Task SurveyCoinEliot(List<Quote> lData)
        {
            try
            {
                var dt = new DateTime(2024, 10, 1, 15,0,0);
                var lTmp = lData.Where(x => (x.Date - dt).TotalDays < 0).ToList();

                var lFrac = lData.GetFractal();
                var isStart = false;
                var i = -1;
                //foreach (var item in lSuperTrend)
                //{
                //    i++;
                //    var itemData = lData.ElementAt(i);
                //    if (!isStart)
                //    {
                //        if (item.UpperBand != null)
                //        {
                //            isStart = true;
                //        }
                //        continue;
                //    }

                //    if (!_flagBuy)
                //    {
                //        if (item.LowerBand != null)
                //        {
                //            PrintBuy(itemData, i, true);
                //            _flagBuy = true;
                //        }
                //        continue;
                //    }

                //    if (_flagBuy)
                //    {
                //        if (item.UpperBand != null)
                //        {
                //            PrintBuy(itemData, i, false);
                //            _flagBuy = false;
                //        }
                //    }
                //}
                //PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveySuperTrend|EXCEPTION| {ex.Message}");
            }
        }
    }
}
