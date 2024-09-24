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
                var lma20 = lData.GetSma(20);
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
                    if (_flagBuy)
                    {
                        var ma20 = lma20.ElementAt(i);
                        var rate = Math.Round(100 * (-1 + cur.Close / _buy.Close), 1);

                        if(cur.Close < (decimal)ma20.Sma && (rate <= -5 || rate >= 5))
                        {
                            PrintBuy(cur, i, false);
                            _flagBuy = false;
                        }
                        if(i == count - 1)//Het danh sach
                        {
                            PrintBuy(cur, i, false);
                            _flagBuy = false;
                        }
                        continue;
                    }


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
                        _flagBuy = true;
                        PrintBuy(cur, i, true);
                    }

                }

                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.VCP|EXCEPTION| {ex.Message}");
            }
        }
    }
}
