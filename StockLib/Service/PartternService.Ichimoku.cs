using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;
using Skender.Stock.Indicators;
using StockLib.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockLib.Service
{
    public partial class PartternService
    {
        public async Task SurveyIchimoku(List<Quote> lData)
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
                    //var itemIchiCur = lIchi.ElementAt(i);

                    var SL_Cur = SL_Indicator(i);
                    var TL_Cur = TL_Indicator(i);
                    var KS_Cur = KS_Indicator(i);
                    var TS_Cur = TS_Indicator(i);
                    var Span1 = Span1_Indicator(i);
                    var Span2 = Span2_Indicator(i);

                    var aboveKumoCur = aboveKumo_Indicator(i, item_Cur);
                    //Kumo Breakout
                    var BuyKumoBreakout = aboveKumoCur && !aboveKumo_Indicator(i - 1, lData[i - 1]) && item_Cur.Close > SL_Cur;
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

            //bool Cross_Indicator(decimal x, decimal y, decimal x_1, decimal y_1)
            //{

            //}
        }
    }
}


//aboveKumo = IIf((C >= Ref(Span1, -t2) AND C >= Ref(Span2, -t2)),1,0);
//withinKumo = IIf((C >= Ref(Span2, -t2) AND C <= Ref(Span1, -t2)),1,0) OR IIf((C>=Ref(Span1,-t2) AND C<=Ref(Span2,-t2)),1,0);
//belowKumo = IIf((C <= Ref(Span1, -t2) AND C <= Ref(Span2, -t2)),1,0);

//BuyStrongTKCross = Cross(TL, SL) AND aboveKumo AND C > TL; 
//BuyStrongPriceCross = Cross(C, SL)AND(C > TL) AND(TL > SL) AND aboveKumo;
//BuyStongSenkouSpanCross = aboveKumo AND Cross(span1,span2);
//BuyStrongChikouSpanCross = aboveKumo AND dlUp AND C > TL;



//public decimal? TenkanSen { get; set; } // conversion line
//public decimal? KijunSen { get; set; } // base line
//public decimal? ChikouSpan { get; set; } // lagging span