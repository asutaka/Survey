using Skender.Stock.Indicators;
using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Survey.TestData
{
    public class Test28122023
    {
        //private static List<Quote> _lBuy = new List<Quote>();
        //private static List<ClsSell> _lSell = new List<ClsSell>();
        public static void MainFunc()
        {
            Console.WriteLine("Coin, Rate Max, Num Max, Rate Min, Num Min");
            var lCoin = Data.GetCoins(5);
            foreach (var item in lCoin)
            {
                Handle(item.symbol.ToLower());
                Thread.Sleep(500);
            }
            //Print();
        }

        private static bool CheckMA4_12(SmaResult param1, SmaResult param2, Quote item = null)
        {
            return (param1.Sma - param2.Sma) > 0 && (item == null || item.Close > item.Open);
        }

        private static bool CheckEMA4_12(EmaResult param1, EmaResult param2, Quote item = null)
        {
            return (param1.Ema - param2.Ema) > 0 && (item == null || item.Close > item.Open);
        }

        private static bool CheckIchimoku(IchimokuResult param, Quote item)
        {
            if (param.SenkouSpanA == null || param.SenkouSpanB == null)
                return false;
            var spanTop = param.SenkouSpanA > param.SenkouSpanB ? param.SenkouSpanA ?? 0 : param.SenkouSpanB ?? 0;
            var spanBottom = param.SenkouSpanA > param.SenkouSpanB ? param.SenkouSpanB ?? 0 : param.SenkouSpanA ?? 0;
            return item.Close > spanBottom && item.Close < spanTop * (decimal)1.03;
        }

        public static void Handle(string coin)
        {
            var lDataQuote = Data.GetData(coin, EInterval.I1H).Select(x => x.To<QuoteEx>());
            var count = lDataQuote.Count();
            var lEma4 = lDataQuote.GetEma(4);
            var lEma12 = lDataQuote.GetEma(12);
            var lIchimoku = lDataQuote.GetIchimoku();
            var lRSI = lDataQuote.GetRsi();

            //Giữ tối đa 24 nến 
            var lSave = new List<QuoteEx>();
            bool hasBuy = false;
            var index = 0;
            decimal minVal = 0, maxVal = 0;
            int indexMin = 0, indexMax = 0;
            QuoteEx elementPrev = null;
            for (int i = 0; i < count; i++)
            {
                if(i > 0)
                {
                    elementPrev = lDataQuote.ElementAt(i - 1);
                }    
                var item = lDataQuote.ElementAt(i);
                var ma4 = lEma4.ElementAt(i);
                var ma12 = lEma12.ElementAt(i);
                if (ma12.Ema is null || ma12.Ema <= 0)
                    continue;

                if(!hasBuy)
                { 
                    var chkMa4_12 = CheckEMA4_12(ma4, ma12);
                    if (chkMa4_12)
                    {
                        var ichimoku = lIchimoku.ElementAt(i);
                        var chkIchimoku = CheckIchimoku(ichimoku, item);
                        if (chkIchimoku)
                        {
                            if (item.Volume < 10 * elementPrev.Volume)
                            {
                                //buy
                                hasBuy = true;
                                lSave.Add(item.To<QuoteEx>());
                                lSave.Last().Coin = coin;
                                minVal = item.Close;
                                maxVal = item.Close;
                            }
                        }
                    }
                }
                else
                {
                    if(++index >= 24)
                    {
                        lSave.Last().Max = maxVal;
                        lSave.Last().SoNenViTriMax = indexMax;
                        lSave.Last().PhanTramMax = 100 * (-1 + Math.Round(maxVal / lSave.Last().Close, 2));
                        lSave.Last().Min = minVal;
                        lSave.Last().SoNenViTriMin = indexMin;
                        lSave.Last().PhanTramMin = 100 * (-1 + Math.Round(minVal / lSave.Last().Close, 2));

                        hasBuy = false;
                        index = 0;
                        maxVal = 0;
                        indexMax = 0;
                        minVal = 0;
                        indexMin = 0;
                    }

                    if(item.High > maxVal)
                    {
                        maxVal = item.High;
                        indexMax = index;
                    }    

                    if(item.Low < minVal)
                    {
                        minVal = item.Low;
                        indexMin = index;
                    }    
                }
            }

            if(hasBuy)
            {
                lSave.Last().Max = maxVal;
                lSave.Last().SoNenViTriMax = indexMax;
                lSave.Last().PhanTramMax = 100 * (-1 + Math.Round(maxVal / lSave.Last().Close, 2));
                lSave.Last().Min = minVal;
                lSave.Last().SoNenViTriMin = indexMin;
                lSave.Last().PhanTramMin = 100 * (-1 + Math.Round(minVal / lSave.Last().Close, 2));
            }
            Print(lSave);
        }

        private static void Print(List<QuoteEx> param)
        {
            foreach (var item in param)
            {
                var print = $"{item.Coin},{item.PhanTramMax}%,{item.SoNenViTriMax},{item.PhanTramMin}%,{item.SoNenViTriMin}";
                //var print = $"COIN: {item.Coin},TIME: {item.Date.ToString("dd/MM/yyyy HH")},BUY: {item.Close},MAX: {item.Max},RATE: {item.PhanTramMax}%,NUM: {item.SoNenViTriMax},MIN: {item.Min},RATE: {item.PhanTramMin}%,NUM: {item.SoNenViTriMin}";
                Console.WriteLine(print);
            }
        }
    }

    
}
