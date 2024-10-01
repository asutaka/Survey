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

        public static bool CheckSuperTrendPharse2(this List<Quote> lData)
        {
            try
            {
                if (!lData.Any())
                    return false;
                //var min = 2;

                var lSuperTrend = lData.GetSuperTrend();
                var last = lSuperTrend.Last();
                var near = lSuperTrend.SkipLast(1).Last();
                var near2 = lSuperTrend.SkipLast(2).Last();
                var near3 = lSuperTrend.SkipLast(3).Last();
                var near4 = lSuperTrend.SkipLast(4).Last();
                var near5 = lSuperTrend.SkipLast(5).Last();

                if (near.LowerBand is null
                    || near2.LowerBand is null
                    || near3.LowerBand is null
                    || near4.LowerBand is null
                    || near5.LowerBand is null)
                    return false;
                if (near2.LowerBand != near3.LowerBand
                    || near2.LowerBand != near4.LowerBand
                    || near2.LowerBand != near5.LowerBand)
                    return false;

                if (near.LowerBand > near2.LowerBand
                    || (near.LowerBand == near2.LowerBand && last.LowerBand > near.LowerBand))
                    return true;
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PartternService.CheckSuperTrendPharse2|EXCEPTION| {ex.Message}");
            }
            return false;
        }
    }
}
