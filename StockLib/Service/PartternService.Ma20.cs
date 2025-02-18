﻿using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyMa20(string code)
        {
            try
            {
                _code = code;
                var stock = StaticVal._lStock.FirstOrDefault(x => x.s == code);
                decimal a = 10;
                if (stock.ex == (int)EExchange.HSX)
                {
                    a = 7;
                }
                else if (stock.ex == (int)EExchange.UPCOM)
                {
                    a = 15;
                }

                var lData = await _apiService.SSI_GetDataStock(code);

                MA20(lData, a);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyMa20|EXCEPTION| {ex.Message}");
            }
        }

        private void MA20(List<Quote> lData, decimal a)
        {
            try
            {
                lData = lData.OrderBy(x => x.Date).ToList();
                var count = lData.Count();
                var lbb = lData.GetBollingerBands();

                for (int i = 50; i < count; i++)
                {
                    var item = lData[i];
                    var bb = lbb.ElementAt(i);
                    if (_flagBuy)
                    {
                        if (_flagRate10)
                        {
                            var rateItem = Math.Round(100 * (-1 + item.Close / item.Open), 2);
                            if (rateItem <= -3
                                || item.Close < (decimal)bb.Sma)
                            {
                                PrintBuy(item, i, false);
                                _flagRate10 = false;
                                _flagBuy = false;
                                continue;
                            }
                        }

                        var rate = Math.Round(100 * (-1 + item.Close / _buy.Close), 1);
                        if (rate >= 10)
                        {
                            _flagRate10 = true;
                        }
                        else if (rate <= -7)
                        {
                            PrintBuy(item, i, false);
                            _flagRate10 = false;
                            _flagBuy = false;
                            continue;
                        }

                        continue;
                    }

                    if (item.Close < item.Open * (decimal)1.01
                        || item.Low >= (decimal)bb.Sma)
                        continue;

                    var bb_check = (item.Low < (decimal)bb.Sma && item.Close > (decimal)bb.Sma && item.High < (decimal)bb.UpperBand && item.Close < item.Open * ((decimal)1 + Math.Round(a / 200, 2)))
                                || (item.Low < (decimal)bb.LowerBand && item.Close > (decimal)bb.LowerBand && item.High < (decimal)bb.Sma);
                    if (!bb_check)
                        continue;

                    var last_check = lData[i - 1].Close < (decimal)lbb.ElementAt(i - 1).Sma;
                    if (!last_check)
                        continue;

                    _flagBuy = true;
                    PrintBuy(item, i, true);
                }

                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.MA20|EXCEPTION| {ex.Message}");
            }
        }
    }

    public static class clsMa20
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
