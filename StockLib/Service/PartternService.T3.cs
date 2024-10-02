using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyT3(string code)
        {
            try
            {
                _code = code;
                var stock = StaticVal._lStock.FirstOrDefault(x => x.s == code);

                var lData = await _apiService.SSI_GetDataStock(code);

                SurveyT3(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyT3|EXCEPTION| {ex.Message}");
            }
        }

        private void SurveyT3(List<Quote> lData)
        {
            try
            {
                var lT3 = lData.GetT3();
                var count = lData.Count();
                var flag = false;

                for (int i = 0; i < count; i++)
                {
                    var item = lData[i];
                    var t3 = lT3.ElementAt(i);
                    if (_flagBuy)
                    {
                        if (item.Close <= (decimal)t3.T3)
                        {
                            PrintBuy(item, i, false);
                            _flagBuy = false;
                        }

                        continue;
                    }

                    if (!flag && item.Close < (decimal)t3.T3)
                    {
                        flag = true;
                        continue;
                    }

                    if (!flag)
                        continue;

                    if(item.Close > (decimal)t3.T3)
                    {
                        _flagBuy = true;
                        PrintBuy(item, i, true);
                    }
                }

                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyT3|EXCEPTION| {ex.Message}");
            }
        }
    }

    public static class clsT3
    {
        //public static bool CheckMa20(this List<Quote> lVal, decimal a)
        //{
        //    try
        //    {
        //        if (!lVal.Any())
        //            return false;
        //        var count = lVal.Count();
        //        var lbb = lVal.GetBollingerBands();
        //        var item = lVal.Last();
        //        var near = lVal.SkipLast(1).TakeLast(1).First();
        //        var bb = lbb.Last();
        //        var bb_near = lbb.SkipLast(1).TakeLast(1).First();
        //        if (item.Close < item.Open * (decimal)1.01
        //                || item.Low >= (decimal)bb.Sma)
        //            return false;

        //        var vol_check = lVal.Skip(count - 11).Take(10).Count(x => item.Volume >= x.Volume * (decimal)1.07) >= 9;
        //        if (!vol_check)
        //            return false;

        //        var bb_check = (item.Low < (decimal)bb.Sma && item.Close > (decimal)bb.Sma && item.High < (decimal)bb.UpperBand && item.Close < item.Open * ((decimal)1 + Math.Round(a / 200, 2)))
        //                    || (item.Low < (decimal)bb.LowerBand && item.Close > (decimal)bb.LowerBand && item.High < (decimal)bb.Sma);
        //        if (!bb_check)
        //            return false;

        //        var last_check = near.Close < (decimal)bb_near.Sma;
        //        if (!last_check)
        //            return false;

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"PartternService.CheckDanZangerCustom|EXCEPTION| {ex.Message}");
        //    }
        //    return false;
        //}
    }
}
