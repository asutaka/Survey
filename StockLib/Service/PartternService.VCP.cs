using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using System.Runtime.Intrinsics.X86;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyVCP(string code)
        {
            try
            {
                _code = code;

                //var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                var lData = await _apiService.SSI_GetDataStock(code);
                VCP(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyVCP|EXCEPTION| {ex.Message}");
            }
        }

        private void VCP(List<Quote> lData)
        {
            try
            {
                var Timeframe = 252;
                var VolTf = 50;
                decimal BaseLowerLimit = 0.6M;
                var PivotLength = 5;
                decimal PVLimit = 0.1M;

                lData = lData.OrderBy(x => x.Date).ToList();
                var lVol = new List<Quote>();
                foreach (var item in lData)
                {
                    lVol.Add(new Quote { Date = item.Date, Close = item.Volume });
                }
                var lma50vol = lVol.GetSma(VolTf);
                var lma50Input = new List<Quote>();
                foreach (var item in lma50vol)
                {
                    lma50Input.Add(new Quote { Date = item.Date, Close = (decimal)(item.Sma ?? 0) });
                }
                var lSlope = lma50Input.GetSlope(VolTf);

                var count = lData.Count();
                if (count < Timeframe)
                    return;

                for (int i = 200; i < count; i++)
                {
                    var cur = lData[i];

                    var lYear = lData.Where(x => x.Date <= cur.Date).TakeLast(Timeframe);
                    if (lYear.Count() < Timeframe)
                        continue;

                    var HighPrice = lYear.MaxBy(x => x.Close);
                    var NearHigh = cur.Close < HighPrice.Close && cur.Close > BaseLowerLimit * HighPrice.Close;
                    var Vma = lma50vol.ElementAt(i).Sma;
                    var VolSlope = lSlope.ElementAt(i).Slope;
                    var VolDecreasing = VolSlope < 0;

                    var PivotHighPrice = lYear.TakeLast(PivotLength).Max(x => x.High);
                    var PivotLowPrice = lYear.TakeLast(PivotLength).Min(x => x.Low);
                    var PivotWidth = Math.Round((PivotHighPrice - PivotLowPrice) / cur.Close, 2);
                    var PivotStartHP = lData.ElementAt(i + 1 - PivotLength).High;
                    var IsPivot = PivotWidth < PVLimit && PivotHighPrice == PivotStartHP;

                    var VolDryUp = true;
                    for (var j = 0; j < PivotLength; j++)
                    {
                        VolDryUp = VolDryUp && lData.ElementAt(i - j).Volume < (decimal)(lma50vol.ElementAt(i - j).Sma ?? 0);
                    }

                    var Results = NearHigh && VolDecreasing && IsPivot && VolDryUp;
                    if(Results)
                    {
                        Console.Write($"{cur.Date.ToString("dd/MM/yyyy")}: BUY");
                    }

                }

                //PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.VCP|EXCEPTION| {ex.Message}");
            }
        }

        //private void VCP(List<Quote> lData)
        //{
        //    try
        //    {
        //        lData = lData.OrderBy(x => x.Date).ToList();

        //        var count = lData.Count();
        //        if (count < 200)
        //            return;

        //        var lma20 = lData.GetSma(20);
        //        var lma50 = lData.GetSma(50);
        //        var lma150 = lData.GetSma(150);
        //        var lma200 = lData.GetSma(200);
        //        var test = 0;

        //        for (int i = 200; i < count; i++)
        //        {
        //            var cur = lData[i];
        //            if(_flagBuy)
        //            {
        //                var rate = (double)Math.Round(-1 + cur.Close / _buy.Close, 2);
        //                if(rate < -0.1)
        //                {
        //                    PrintBuy(cur, i, false);
        //                    _flagBuy = false;
        //                }
        //                var ma20 = lma20.ElementAt(i);
        //                if(cur.Close < (decimal)ma20.Sma)
        //                {
        //                    PrintBuy(cur, i, false);
        //                    _flagBuy = false;
        //                }
        //                continue;
        //            }

        //            var lYear = lData.Take(i).Where(x => x.Date >= cur.Date.AddYears(-1));
        //            if (lYear.Count() < 200)
        //                continue;

        //            var max = lYear.MaxBy(x => x.Close);
        //            var min = lYear.MinBy(x => x.Close);
        //            var minFromMax = lData.Where(x => x.Date > max.Date && x.Date <= cur.Date).MinBy(x => x.Close);
        //            var divMinMax = (minFromMax.Date - max.Date).TotalDays;
        //            if (divMinMax < 0
        //                || cur.Close < min.Close * (decimal)1.3
        //                || cur.Close < max.Close * (decimal)0.75) 
        //                continue;

        //            var ma50 = lma50.ElementAt(i);
        //            var ma150 = lma150.ElementAt(i);
        //            var ma200 = lma200.ElementAt(i);
        //            if ((double)cur.Close <= Math.Max(Math.Max(ma50.Sma ?? 0, ma150.Sma ?? 0), ma200.Sma ?? 0)) 
        //                continue;

        //            if (ma50.Sma <= Math.Max(ma150.Sma ?? 0, ma200.Sma ?? 0)
        //                || ma150.Sma < ma200.Sma) 
        //                continue;

        //            var ma200_month = lma200.ElementAt(i - 20);
        //            if (ma200.Sma <= (ma200_month.Sma ?? 0) * 1.1)
        //                continue;

        //            Console.WriteLine($"Test: {test++}| {cur.Date.ToString("dd/MM/yyyy")}");

        //            var divTime = (cur.Date - max.Date).TotalDays;
        //            if (divTime < 30 || divTime < divMinMax * 2) 
        //                continue;
                   

        //            var lPart1 = lData.Where(x => x.Date >= max.Date && x.Date <= max.Date.AddDays(divMinMax * 2));
        //            var lPart2 = lData.Where(x => x.Date > max.Date.AddDays(divMinMax * 2) && x.Date <= cur.Date);

        //            //if (lPart1.Average(x => x.Volume) <= lPart2.Average(x => x.Volume))
        //            //    continue;
        //            var minPart1 = lPart1.MinBy(x => x.Close);
        //            var minPart2 = lPart2.MinBy(x => x.Close);
        //            if (minPart1.Close < min.Close * (decimal)1.4
        //                || minPart2.Close < minPart1.Close * (decimal)1.1)
        //                continue;

        //            if (!_flagBuy)
        //            {
        //                _flagBuy = true;
        //                PrintBuy(cur, i, true);
        //            }
        //        }

        //        PrintBuyLast();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"PartternService.VCP|EXCEPTION| {ex.Message}");
        //    }
        //}
    }
}
