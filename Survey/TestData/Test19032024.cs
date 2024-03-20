using Skender.Stock.Indicators;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Survey.TestData
{
    public class Test19032024
    {
        public static void MainFunc()
        {
            Console.WriteLine("Coin, Rate Max, Num Max, Rate Min, Num Min");
            var lCoin = Data.GetCoins(5);
            Test2("btcusdt");
        }

        public static void print(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void Test2(string coin)
        {
            var lData = Data.GetDataAll(coin, EInterval.I4H);
            var lDataQuote = lData.Select(x => x.To<Quote>());
            var lMA5 = lDataQuote.GetEma(5);
            var lMA12 = lDataQuote.GetEma(12);
            var count = lMA12.Count();
            var state = false;
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);
                var item5 = lMA5.ElementAt(i);
                var item12 = lMA12.ElementAt(i);
                if(!state && item5.Ema >= item12.Ema)
                {
                    state = true;
                }
                else if(state && item5.Ema < item12.Ema)
                {
                    state = false;
                }
                else
                {
                    continue;
                }
                if(state)
                {
                    print($"Date: {item.Date.ToString("dd/MM/yyyy HH")}");
                }
            }

            //var count = lDataQuote.Count();
            //var lIchimoku = lDataQuote.GetIchimoku();
            //var lRSI = lDataQuote.GetRsi();
            //var state = 0;

            ////Dem toi thieu 15 cay nen duoi ichimoku thi active
            //DateTime dtBuy = new DateTime();
            //var lBuy = new List<Quote>();
            //var lSell = new List<ClsSell>();
            //var countIchi = 0;
            //decimal maxClose = 0;
            //var count24 = 0;
            //var iFirst = 0;
            //var iLast = 0;
            //for (int i = 0; i < count; i++)
            //{
            //    var item = lDataQuote.ElementAt(i);
            //    var ichimoku = lIchimoku.ElementAt(i);
            //    if (ichimoku.SenkouSpanA is null)
            //        continue;
            //    var spanTop = ichimoku.SenkouSpanA > ichimoku.SenkouSpanB ? ichimoku.SenkouSpanA : ichimoku.SenkouSpanB;
            //    var spanBottom = ichimoku.SenkouSpanA > ichimoku.SenkouSpanB ? ichimoku.SenkouSpanB : ichimoku.SenkouSpanA;
            //    var PriceTop = item.Close > item.Open ? item.Close : item.Open;
            //    var PriceBottom = item.Close > item.Open ? item.Open : item.Close;
            //    var rsi = lRSI.ElementAt(i)?.Rsi ?? 0;

            //    if (state == 0)//idle
            //    {
            //        if (countIchi >= 15
            //            && item.Close > item.Open
            //            && PriceBottom < spanTop
            //            && PriceTop > spanTop)
            //        {
            //            state = 1;
            //            continue;
            //        }

            //        if (PriceTop < spanTop && PriceBottom < spanBottom)
            //        {
            //            countIchi++;
            //        }
            //        else
            //        {
            //            countIchi = 0;
            //        }
            //    }
            //    else
            //    {
            //        if (state == 1)
            //        {
            //            lBuy.Add(item);
            //            state = 2;
            //            maxClose = item.Close;
            //            iFirst = i;
            //            iLast = i;
            //            dtBuy = item.Date;
            //            continue;
            //        }

            //        if (maxClose < PriceTop)
            //        {
            //            maxClose = PriceTop;
            //            iLast = i;
            //        }
            //        count24++;

            //        if (count24 >= 24)
            //        {
            //            lSell.Add(new ClsSell { Sell = maxClose, Count = iLast - iFirst, Coin = coin, DateBuy = dtBuy });
            //            state = 0;
            //            maxClose = 0;
            //            count24 = 0;
            //            countIchi = 0;
            //            iFirst = 0;
            //            iLast = 0;
            //        }
            //    }
            //}
            //if (maxClose > 0)
            //    lSell.Add(new ClsSell { Sell = maxClose, Count = count - iFirst, Coin = coin, DateBuy = dtBuy });

            //_lBuy.AddRange(lBuy);
            //_lSell.AddRange(lSell);
        }
    }
}
