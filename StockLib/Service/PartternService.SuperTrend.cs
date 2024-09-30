using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveySuperTrend(string code)
        {
            try
            {
                _code = code;

                //var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                var lData = await _apiService.SSI_GetDataStock(code);
                await SurveySuperTrend(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveySuperTrend|EXCEPTION| {ex.Message}");
            }
        }
        private async Task SurveySuperTrend(List<Quote> lData)
        {
            try
            {
                var lSuperTrend = lData.GetSuperTrend();
                var isStart = false;
                var i = -1;
                foreach (var item in lSuperTrend)
                {
                    i++;
                    var itemData = lData.ElementAt(i);
                    if (!isStart)
                    {
                        if(item.UpperBand != null)
                        {
                            isStart = true;
                        }
                        continue;
                    }

                    if(!_flagBuy)
                    {
                        if(item.LowerBand != null)
                        {
                            PrintBuy(itemData, i, true);
                            _flagBuy = true;
                        }
                        continue;
                    }

                    if(_flagBuy)
                    {
                        if(item.UpperBand != null)
                        {
                            PrintBuy(itemData, i, false);
                            _flagBuy = false;
                        }
                    }
                }
                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveySuperTrend|EXCEPTION| {ex.Message}");
            }
        }
    }

    public static class clsSuperTrend
    {
        public static bool CheckSuperTrend(this List<Quote> lData)
        {
            try
            {
                if (!lData.Any())
                    return false;
                var lSuperTrend = lData.GetSuperTrend();
                var last = lSuperTrend.Last();
                var near = lSuperTrend.SkipLast(1).Last();

                return (last.LowerBand != null && near.LowerBand is null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PartternService.CheckSuperTrend|EXCEPTION| {ex.Message}");
            }
            return false;
        }
    }
}
