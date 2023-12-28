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

        private static bool CheckRSI(RsiResult param, Quote item = null)
        {
            return (param.Rsi > 30 && param.Rsi < 70) && (item == null || item.Close > item.Open);
        }

        private static bool CheckMA4_12(SmaResult param1, SmaResult param2, Quote item = null)
        {
            return (param1.Sma - param2.Sma) > 0 && (item == null || item.Close > item.Open);
        }

        private static bool CheckEMA4_12(EmaResult param1, EmaResult param2, Quote item = null)
        {
            return (param1.Ema - param2.Ema) > 0 && (item == null || item.Close > item.Open);
        }

        private static bool CheckIchimoku(IchimokuResult param, Quote item = null)
        {
            if (param.SenkouSpanA == null || param.SenkouSpanB == null)
                return false;
            var spanTop = param.SenkouSpanA > param.SenkouSpanB ? param.SenkouSpanA : param.SenkouSpanB;
            var spanBottom = param.SenkouSpanA > param.SenkouSpanB ? param.SenkouSpanB : param.SenkouSpanA;
            return item.Close > spanTop && item.Open < spanTop && item.Open > spanBottom && (item == null || item.Close > item.Open);
        }

        private static bool CheckMacd(MacdResult param, Quote item = null)
        {
            return param.Macd > param.Signal && (item == null || item.Close > item.Open);
        }

        public static void Handle(string coin)
        {
            var lDataQuote = Data.GetData(coin, EInterval.I1H).Select(x => x.To<QuoteEx>());
            var count = lDataQuote.Count();
            var lMa4 = lDataQuote.GetSma(4);
            var lMa12 = lDataQuote.GetSma(12);
            var lEma4 = lDataQuote.GetEma(4);
            var lEma12 = lDataQuote.GetEma(12);
            var lMacd = lDataQuote.GetMacd();
            var lIchimoku = lDataQuote.GetIchimoku();
            var lRSI = lDataQuote.GetRsi();

            //Giữ tối đa 24 nến và 48 nến
            var lSave = new List<QuoteEx>();
            bool hasBuy = false;
            var index = 0;
            decimal minVal = 0, maxVal = 0;
            int indexMin = 0, indexMax = 0;
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);
                var ma4 = lMa4.ElementAt(i);
                var ma12 = lMa12.ElementAt(i);
                if (ma12.Sma is null || ma12.Sma <= 0)
                    continue;

                if(!hasBuy)
                {
                    var chkMa4_12 = CheckMA4_12(ma4, ma12);
                    if (chkMa4_12)
                    {
                        //buy
                        hasBuy = true;
                        lSave.Add(item.To<QuoteEx>());
                        lSave.Last().Coin = coin;
                        minVal = item.Close;
                        maxVal = item.Close;
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
