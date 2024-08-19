 using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyIchimoku(string code)
        {
            try
            {
                var lData = await _apiService.SSI_GetDataStock(code);
                await SurveyIchimoku(lData);
            }
            catch(Exception ex)
            {
                _logger.LogError($"PartternService.SurveyIchimoku|EXCEPTION| {ex.Message}");
            }
        }
        private async Task SurveyIchimoku(List<Quote> lData)
        {
            int t1 = 12, t2 = 26, t3 = 65, t4 = 129;
            try
            {
                lData = lData.OrderBy(x => x.Date).ToList();
                var count = lData.Count();
                if (count < t4)
                    return;

                for (int i = t4 - 1; i < count; i++)
                {
                    var item_Cur = lData[i];

                    var SL_Cur = SL_Indicator(i);
                    var TL_Cur = TL_Indicator(i);
                    var KS_Cur = KS_Indicator(i);
                    var TS_Cur = TS_Indicator(i);
                    var Span1 = Span1_Indicator(i);
                    var Span2 = Span2_Indicator(i);

                    var aboveKumo_Cur = aboveKumo_Indicator(i, item_Cur);
                    var dlUp = lData[i - (t2 + 1)].Close > item_Cur.Close && lData[i - t2].Close < item_Cur.Close && item_Cur.Close > lData[i - 1].Close;
                    //Kumo Breakout
                    var BuyKumoBreakout = aboveKumo_Cur && !aboveKumo_Indicator(i - 1, lData[i - 1]) && item_Cur.Close > SL_Cur;
                    //TK Cross
                    var BuyStrongTKCross = Cross_Indicator(TL_Cur, SL_Cur, TL_Indicator(i - 1), SL_Indicator(i - 1)) && aboveKumo_Cur && item_Cur.Close > TL_Cur;
                    //Price Cross
                    var BuyStrongPriceCross = Cross_Indicator(item_Cur.Close, SL_Cur, lData[i - 1].Close, SL_Indicator(i - 1)) && item_Cur.Close > TL_Cur && TL_Cur > SL_Cur && aboveKumo_Cur;
                    //SenkouSpan Cross
                    var BuyStongSenkouSpanCross = aboveKumo_Cur && Cross_Indicator(Span1, Span2, Span1_Indicator(i - 1), Span2_Indicator(i - 1));
                    //ChikouSpan Cross
                    var BuyStrongChikouSpanCross = aboveKumo_Cur && dlUp && item_Cur.Close > TL_Cur;

                    var content = string.Empty;
                    if(BuyKumoBreakout)
                    {
                        content += "Kumo Breakout,";
                    }
                    if (BuyStrongTKCross)
                    {
                        content += "TK Cross,";
                    }
                    if (BuyStrongPriceCross) 
                    {
                        content += "Price Cross,";
                    }
                    if (BuyStongSenkouSpanCross)
                    {
                        content += "SenkouSpan Cross,";
                    }
                    if (BuyStrongChikouSpanCross)
                    {
                        content += "ChikouSpan Cross,";
                    }
                    if(!string.IsNullOrWhiteSpace(content))
                    {
                        Console.WriteLine($"{item_Cur.Date.ToString("dd/MM/yyyy")}:{content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartternService.SurveyIchimoku|EXCEPTION| {ex.Message}");
            }

            decimal SL_Indicator(int i)
            {
                var SL = (lData.Skip(i - 17).Take(17).Max(x => x.High) + lData.Skip(i - 17).Take(17).Min(x => x.Low)) / 2;
                return SL;
            }

            decimal TL_Indicator(int i)
            {
                var TL = (lData.Skip(i - 9).Take(9).Max(x => x.High) + lData.Skip(i - 9).Take(9).Min(x => x.Low)) / 2;
                return TL;
            }

            decimal KS_Indicator(int i)
            {
                var KS = (lData.Skip(i - 65).Take(65).Max(x => x.High) + lData.Skip(i - 65).Take(65).Min(x => x.Low)) / 2;
                return KS;
            }

            decimal TS_Indicator(int i)
            {
                var TS = (lData.Skip(i - 129).Take(129).Max(x => x.High) + lData.Skip(i - 129).Take(129).Min(x => x.Low)) / 2;
                return TS;
            }

            decimal Span1_Indicator(int i)
            {
                var SL = (lData.Skip(i - 17).Take(17).Max(x => x.High) + lData.Skip(i - 17).Take(17).Min(x => x.Low)) / 2;
                var TL = (lData.Skip(i - 9).Take(9).Max(x => x.High) + lData.Skip(i - 9).Take(9).Min(x => x.Low)) / 2;
                var Span1 = (SL + TL) / 2;
                return Span1;
            }

            decimal Span2_Indicator(int i)
            {
                var Span2 = (lData.Skip(i - t2).Take(t2).Max(x => x.High) + lData.Skip(i - t2).Take(t2).Min(x => x.Low)) / 2;
                return Span2;
            }

            bool aboveKumo_Indicator(int i, Quote element)
            {
                var aboveKumoCur = element.Close >= Math.Max(Span1_Indicator(i - t2), Span2_Indicator(i - t2));
                return aboveKumoCur;
            }

            bool Cross_Indicator(decimal x, decimal y, decimal x_1, decimal y_1)
            {
                return x > y && x_1 <= y_1;
            }
        }
    }
}