using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveySuperTrend(string code)
        {
            try
            {
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

                for (int i = 10; i < count; i++)
                {
                    trend[i] = 1;
                    TrendUp[i] = 0;
                    TrendDown[i] = 0;
                    if (i < 10)
                    {
                        continue;
                    }
                    var item = lData[i];

                    var iATR = (decimal)(lAtr.ElementAt(i).Atr ?? 0);
                    var Up = Up_Indicator(i);
                    var Dn = Dn_Indicator(i);


                    if (item.Close > Up_Indicator(i - 1))
                    {
                        trend[i] = 1;
                        if (trend[i - 1] == -1)
                        {
                            changeOfTrend = 1;
                        }
                    }
                    else if (item.Close < Dn_Indicator(i - 1)) 
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

                    if(trend[i]>0 && Dn < Dn_Indicator(i - 1))
                    {
                        Dn = Dn_Indicator(i - 1);
                    }

                    if(trend[i]<0 && Up > Up_Indicator(i - 1))
                    {
                        Up = Up_Indicator(i - 1);
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

                    //Add
                    if (trend[i] == 1)
                    {
                        Console.WriteLine($"Buy {item.Date.ToString("dd/MM/yyyy")}");
                    }
                    else if (trend[i] == -1) 
                    {
                        Console.WriteLine($"Sell {item.Date.ToString("dd/MM/yyyy")}");
                    }
                }

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
