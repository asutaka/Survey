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
    public class Test25122023
    {
        private static List<Quote> _lBuy = new List<Quote>();
        private static List<ClsSell> _lSell = new List<ClsSell>();
        public static void Test1()
        {
            var lCoin = Data.GetCoins(150);

            foreach (var item in lCoin)
            {
                Test2(item.symbol.ToLower());
                Thread.Sleep(500);
            }
            Print();
        }

        public static void Test2(string coin)
        {
            var lData = Data.GetData(coin, EInterval.I1H);
            var lDataQuote = lData.Select(x => x.To<Quote>());
            var count = lDataQuote.Count();
            var lIchimoku = lDataQuote.GetIchimoku();
            var lRSI = lDataQuote.GetRsi();
            var state = 0;

            //Dem toi thieu 15 cay nen duoi ichimoku thi active
            DateTime dtBuy = new DateTime();
            var lBuy = new List<Quote>();
            var lSell = new List<ClsSell>();
            var countIchi = 0;
            decimal maxClose = 0;
            var count24 = 0;
            var iFirst = 0;
            var iLast = 0;
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);
                var ichimoku = lIchimoku.ElementAt(i);
                if (ichimoku.SenkouSpanA is null)
                    continue;
                var spanTop = ichimoku.SenkouSpanA > ichimoku.SenkouSpanB ? ichimoku.SenkouSpanA : ichimoku.SenkouSpanB;
                var spanBottom = ichimoku.SenkouSpanA > ichimoku.SenkouSpanB ? ichimoku.SenkouSpanB : ichimoku.SenkouSpanA;
                var PriceTop = item.Close > item.Open ? item.Close : item.Open;
                var PriceBottom = item.Close > item.Open ? item.Open : item.Close;
                var rsi = lRSI.ElementAt(i)?.Rsi ?? 0;

                if (state == 0)//idle
                {
                    if(countIchi >= 15
                        && item.Close > item.Open
                        && PriceBottom < spanTop
                        && PriceTop > spanTop)
                    {
                        state = 1;
                        continue;
                    }    

                    if (PriceTop < spanTop && PriceBottom < spanBottom)
                    {
                        countIchi++;
                    }
                    else
                    {
                        countIchi = 0;
                    }
                }
                else
                {
                    if(state == 1)
                    {
                        lBuy.Add(item);
                        state = 2;
                        maxClose = item.Close;
                        iFirst = i;
                        iLast = i;
                        dtBuy = item.Date;
                        continue;
                    }

                    if (maxClose < PriceTop)
                    {
                        maxClose = PriceTop;
                        iLast = i;
                    }
                    count24++;

                    if(count24 >= 24)
                    {
                        lSell.Add(new ClsSell { Sell = maxClose, Count = iLast - iFirst, Coin = coin, DateBuy = dtBuy });
                        state = 0;
                        maxClose = 0;
                        count24 = 0;
                        countIchi = 0;
                        iFirst = 0;
                        iLast = 0;
                    }    
                }
            }
            if (maxClose > 0)
                lSell.Add(new ClsSell { Sell = maxClose, Count = count - iFirst, Coin = coin, DateBuy = dtBuy });

            _lBuy.AddRange(lBuy);
            _lSell.AddRange(lSell);
        }

        private static void Print()
        {
            //print
            var countResult = _lSell.Count();
            var lRate = new List<decimal>();
            _lBuy = _lBuy.OrderBy(x => x.Date).ToList();
            _lSell = _lSell.OrderBy(x => x.DateBuy).ToList();

            for (int i = 0; i < countResult; i++)
            {
                var itemBuy = _lBuy.ElementAt(i);
                var itemSell = _lSell.ElementAt(i);
                var PriceBuy = itemBuy.Open;
                var PriceSell = itemSell.Sell;
                var rate = Math.Round((PriceSell - PriceBuy) * 100 / PriceBuy, 2);
                lRate.Add(rate);

                var print = $"Coin: {itemSell.Coin};BUY: {itemBuy.Date.ToString("dd/MM/yyyy HH:mm")};PRICE_B: {PriceBuy} | PRICE_S: {PriceSell}| Candles: {itemSell.Count} | RATE: {rate}%";
                //var print = $"BUY: {itemBuy.Date.ToString("dd/MM/yyyy HH:mm")};PRICE_B: {PriceBuy} | SELL: {itemSell.DateTimeStamp.ToString("dd/MM/yyyy HH:mm")}; DES: {itemSell.Description}; PRICE_S: {PriceSell} | RATE: {rate}%";
                Console.WriteLine(print);
            }
        }
    }

    public class ClsSell
    {
        public DateTime DateBuy { get; set; }
        public decimal Sell { get; set; }
        public int Count { get; set; }
        public string Coin { get; set; }
    }
}
