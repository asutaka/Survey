using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyW(string code)
        {
            try
            {
                _code = code;

                //var lData = await _apiService.SSI_GetDataStock_Alltime(code);
                var lData = await _apiService.SSI_GetDataStock(code);
                W(lData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyW|EXCEPTION| {ex.Message}");
            }
        }

        private void W(List<Quote> lData)
        {
            try
            {
                decimal percdiff = 5;
                decimal fwdcheck = 5;
                decimal mindistance = 10;
                decimal validdiff = percdiff / 400;

                var lZigZag = lData.GetZigZag();
                var lPK = new List<int>();
                var lTR = new List<int>();

                var index = 0;
                foreach (var item in lZigZag)
                {
                    if(item.PointType == "H")
                        lPK.Add(index);
                    else if(item.PointType == "L")
                        lTR.Add(index);
                    index++;
                }
                lPK.Reverse();
                lTR.Reverse();
                var countPK = lPK.Count;
                var countTR = lTR.Count;

                if (countPK <= 1 || countTR <= 1)
                    return;

                var lPeakDiff = new List<decimal>();
                for (int i = 0; i < countPK - 1; i++) 
                {
                    var xpk1_val = lData.ElementAt(lPK[i]).High;
                    var xpk2_val = lData.ElementAt(lPK[i + 1]).High;
                    lPeakDiff.Add(xpk1_val / xpk2_val);
                }

                var lTroughdiff = new List<decimal>();
                for (int i = 0; i < countPK - 1; i++)
                {
                    var xtr1 = lData.ElementAt(lTR[i]).Low;
                    var xtr2 = lData.ElementAt(lTR[i + 1]).Low;
                    lTroughdiff.Add(xtr1 / xtr2);
                }

                var lDoubleTop = new List<bool>();
                for (int i = 0; i < countPK - 2; i++)
                {
                    var peakDiff = lPeakDiff[i];
                    var xpk1 = lPK[i];
                    var xpk2 = lPK[i + 1];
                    var hhv = lData.Skip(xpk1 + 1).Take((int)fwdcheck).Max(x => x.High);
                    var doubleTop = (Math.Abs(peakDiff - 1) < validdiff)
                            && ((xpk1 - xpk2) > mindistance)
                            && lData.ElementAt(xpk1).High >= hhv;
                    lDoubleTop.Add(doubleTop);
                }

                var lDoubleBot = new List<bool>();
                for (int i = 0; i < countTR - 2; i++)
                {
                    var troughdiff = lTroughdiff[i];
                    var xtr1 = lTR[i];
                    var xtr2 = lTR[i + 1];
                    var llv = lData.Skip(xtr1 + 1).Take((int)fwdcheck).Min(x => x.Low);

                    var doubleBot = (Math.Abs(troughdiff - 1) < validdiff)
                            && ((xtr1 - xtr2) > mindistance)
                            && lData.ElementAt(xtr1).Low <= llv;
                    if(doubleBot)
                    {
                        var tmp = 1;
                    }
                    lDoubleBot.Add(doubleBot);
                }

                var lResult = new List<Quote>();
                for (int i = 0; i < countPK - 3; i++)
                {
                    var doubleTop = lDoubleTop[i];
                    if (doubleTop)
                        lResult.Add(lData.ElementAt(lPK[i]));
                }
                for (int i = 0; i < countTR - 3; i++)
                {
                    var doubleBot = lDoubleBot[i];
                    if (doubleBot)
                        lResult.Add(lData.ElementAt(lTR[i]));
                }
                lResult = lResult.OrderBy(x => x.Date).ToList();//Quá ít tín hiệu

                PrintBuyLast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.VCP|EXCEPTION| {ex.Message}");
            }
        }
    }
}
