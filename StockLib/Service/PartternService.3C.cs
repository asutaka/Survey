using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task Survey3C(string code)
        {
            try
            {
                _code = code;

                //var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                var lData = await _apiService.SSI_GetDataStock(code);
                CupCheat(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.Survey3C|EXCEPTION| {ex.Message}");
            }
        }

        private void CupCheat(List<Quote> lData)
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
                var lma200 = lData.GetSma(200);
                var lZigZag = lData.GetZigZag();
                var lPK = new List<Quote>();
                var lTR = new List<Quote>();

                var index = 0;
                foreach (var item in lZigZag)
                {
                    if (item.PointType == "H")
                        lPK.Add(lData.ElementAt(index));
                    else if (item.PointType == "L")
                        lTR.Add(lData.ElementAt(index));
                    index++;
                }
                lPK.Reverse();
                lTR.Reverse();
                var countPK = lPK.Count;
                var countTR = lTR.Count;

                if (countPK <= 0 || countTR <= 0)
                    return;

                var count = lData.Count();
                if (count < Timeframe)
                    return;

                for (int i = 200; i < count; i++)
                {
                    var cur = lData[i];
                    if (_flagBuy)
                    {
                        if (_flagRate10)
                        {
                            var rateItem = Math.Round(100 * (-1 + cur.Close / cur.Open));
                            if (rateItem <= -3
                                || cur.Close < (decimal)lma20.ElementAt(i).Sma)
                            {
                                PrintBuy(cur, i, false);
                                _flagRate10 = false;
                                _flagBuy = false;
                                continue;
                            }
                        }

                        var rate = Math.Round(100 * (-1 + cur.Close / _buy.Close), 1);
                        if (rate >= 10)
                        {
                            _flagRate10 = true;
                        }
                        else if (rate <= -7)
                        {
                            PrintBuy(cur, i, false);
                            _flagRate10 = false;
                            _flagBuy = false;
                            continue;
                        }

                        continue;
                    }

                    var peak = lPK.Where(x => x.Date >= cur.Date.AddYears(-1) && x.Date < cur.Date).MaxBy(x => x.Close);
                    if(peak is null)
                        continue;

                    var trough = lTR.Where(x => x.Date > peak.Date && x.Date < cur.Date).MinBy(x => x.Close);
                    if (trough is null)
                        continue;

                    var checkParttern = peak.Close > trough.Close * (decimal)1.15 && peak.Close < trough.Close * (decimal)1.5;
                    if (!checkParttern)
                        continue;

                    var checkDay = (cur.Date - trough.Date).TotalDays >= 10;
                    if (!checkDay)
                        continue;

                    var ma200 = lma200.ElementAt(i);
                    if (cur.Close <= (decimal)ma200.Sma)
                        continue;

                    var distanceRate = Math.Round(100 * (-1 + peak.Close / trough.Close), 1);
                    var lNear = lData.Skip(i - 5).Take(5);
                    var minNear = lNear.Min(x => x.Close);
                    var maxNear = lNear.Max(x => x.Close);
                    var nearRate = Math.Round(100 * (-1 + maxNear / minNear), 1);
                    if (nearRate > (decimal)0.3 * distanceRate)
                        continue;

                    var curRate = Math.Round(100 * (-1 + cur.Close / cur.Open), 1);
                    if (curRate < 2)
                        continue;

                    var vol_check = lData.Skip(i - 10).Take(10).Count(x => cur.Volume >= x.Volume * (decimal)1.02) >= 9;
                    if (!vol_check)
                        continue;

                    _flagBuy = true;
                    PrintBuy(cur, i, true);
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
