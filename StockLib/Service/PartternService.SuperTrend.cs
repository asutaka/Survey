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
                var lData = await _apiService.SSI_GetDataStock(code);
                lData = lData.Take(lData.Count() - 1).TakeLast(200).ToList();
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
                lData = lData.OrderBy(x => x.Date).ToList();
                var lAtr = lData.GetAtr(10);
                var count = lData.Count();
                var Factor = 4;
                var Pd = 10;
                decimal[] TrendUp = new decimal[count], TrendDown = new decimal[count];
                int[] trend = new int[count];
                trend[0] = 1;
                var changeOfTrend = 0;
                var flag = 0;
                var flagh = 0;
                var lUp = new List<decimal?>
                {
                    null
                };

                var lDown = new List<decimal?>
                {
                    null
                };

                for (int i = 1; i < count; i++)
                {
                    trend[i] = 1;
                    TrendUp[i] = 0;
                    TrendDown[i] = 0;
                    var item = lData[i];


                    //if(i == 108)
                    //{
                    //    var tmp = 1;
                    //}    
                    //if(item.Date.Year == 2024 && item.Date.Month == 5 && item.Date.Day == 15)
                    //{
                    //    var tmp = 1;
                    //}

                    var iATR = (decimal)(lAtr.ElementAt(i).Atr ?? 0);
                    var Up = Up_Indicator(i);
                    var Dn = Dn_Indicator(i);


                    if (item.Close > (lUp[i-1] ?? 0)) 
                    {
                        trend[i] = 1;
                        if (trend[i - 1] == -1)
                        {
                            changeOfTrend = 1;
                        }
                    }
                    else if (item.Close < (lDown[i - 1] ?? 0)) 
                    {
                        trend[i] = -1;
                        if(trend[i-1] == 1)
                        {
                            changeOfTrend = 1;
                        }
                    }
                    else if(trend[i-1] == 1)
                    {
                        trend[i] = 1;
                        changeOfTrend = 0;
                    }
                    else if(trend[i-1] == -1)
                    {
                        trend[i] = -1;
                        changeOfTrend = 0;
                    }

                    flag = (trend[i] < 0 && trend[i - 1] > 0) ? 1 : 0;
                    flagh = (trend[i] > 0 && trend[i - 1] < 0) ? 1 : 0;

                    if(trend[i]>0 && Dn < (lDown[i - 1] ?? 0))
                    {
                        Dn = (lDown[i - 1] ?? 0);
                    }

                    if(trend[i]<0 && Up > (lUp[i - 1] ?? 0))
                    {
                        Up = (lUp[i - 1] ?? 0);
                    }

                    if(flag == 1)
                    {
                        Up = (item.High + item.Low) / 2 + (Factor * iATR);
                    }

                    if(flagh == 1)
                    {
                        Dn = (item.High + item.Low) / 2 - (Factor * iATR);
                    }

                    if(trend[i] == 1)
                    {
                        TrendUp[i] = Dn;
                        if(changeOfTrend == 1)
                        {
                            TrendUp[i - 1] = TrendDown[i - 1];
                            changeOfTrend = 0;
                        }
                    }
                    else if(trend[i] == -1)
                    {
                        TrendDown[i] = Up;
                        if(changeOfTrend == 1)
                        {
                            TrendDown[i - 1] = TrendUp[i - 1];
                            changeOfTrend = 0;
                        }
                    }

                    lUp.Add(Up);
                    lDown.Add(Dn);

                    if (!_flagBuy && trend[i] == 1)
                    {
                        _flagBuy = true;
                        PrintBuy(item, i, true);
                    }
                    else if (_flagBuy && trend[i] == -1)
                    {
                        PrintBuy(item, i, false);
                        _flagBuy = false;
                    }
                }

                PrintBuyLast();

                decimal Up_Indicator(int index)
                {
                    var Up = (lData[index].High + lData[index].Low) / 2 + (Factor * (decimal)(lAtr.ElementAt(index).Atr ?? 0));
                    return Up;
                }

                decimal Dn_Indicator(int index)
                {
                    var Dn = (lData[index].High + lData[index].Low) / 2 - (Factor * (decimal)(lAtr.ElementAt(index).Atr ?? 0));
                    return Dn;
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveySuperTrend|EXCEPTION| {ex.Message}");
            }
        }
    }
}
