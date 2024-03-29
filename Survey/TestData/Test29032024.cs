using Skender.Stock.Indicators;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.TestData
{
    public class Test29032024
    {
        public static void MainFunc()
        {
            Console.WriteLine("Coin, Rate Max, Num Max, Rate Min, Num Min");
            Test2("btcusdt");
            print();
        }

        public static void print()
        {
            //var tmp = _lResult.Count(x => x.DoDaiNen >= 9);
            //var rate = tmp * 100 / _lResult.Count();
            //Console.WriteLine(rate);
            //_lResult = _lResult.Where(x => x.DoDaiNen >= 8).ToList();


            //foreach (var item in _lResult)
            //{
            //    Console.WriteLine($"Ngay: {item.Ngay.ToString("dd/MM/yyyy")}\tDo dai nen: {item.DoDaiNen}%\t" +
            //                    $"Index UP: {item.IndexUp}; Rate UP: {item.RateUp}%\t" +
            //                    $"Index DOWN: {item.IndexDown}; Rate DOWN: {item.RateDown}%");
            //}
        }

        public static List<cls25032024> _lResult = new List<cls25032024>();
        private static Quote _ItemCheckCur = null;
        private static bool _check2Sell = false;
        private static bool Sell(Quote item, EmaResult ema5, EmaResult ema12, BollingerBandsResult bb)
        {
            if (!_check2Sell)
                return false;

            /*
             * Tín hiệu Bán
                - case 1: ema cắt xuống bán luôn 
                - case 2: giá dưới Ma20 bán
                - case 3: thỏa 2 điều kiện trên  
                
                Tín hiệu cutloss:
                - Giá giảm xuống Close cuả nến tín hiệu
             */

            //CASE 1: 
            if (ema5.Ema < ema12.Ema)
                return true;

            ////CASE 2: 
            //if (item.Close < (decimal)bb.Sma)
            //    return true;

            ////CASE 3: 
            //if (ema5.Ema < ema12.Ema
            //    && (item.Close < (decimal)bb.Sma))
            //    return true;

            //CUTLOSS
            if (item.Close < _ItemCheckCur.Close)
                return true;

            return false;
        }
        public static void Test2(string coin)
        {
            var lDataQuote = Data.GetDataAll(coin, EInterval.I1D).Select(x => x.To<Quote>());
            var lEMA5 = lDataQuote.GetEma(5);
            var lEMA12 = lDataQuote.GetEma(12);
            var lRsi = lDataQuote.GetRsi();
            var lBB = lDataQuote.GetBollingerBands();
            var count = lDataQuote.Count();

            List<Quote> lSig = new List<Quote>();
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);

                if (lEMA5.ElementAt(i).Ema is null
                    || lEMA12.ElementAt(i).Ema is null
                    || lRsi.ElementAt(i).Rsi is null
                    || lBB.ElementAt(i).LowerBand is null
                )
                    continue;

                if(item.Close > item.Open
                    && item.Close > (decimal)lBB.ElementAt(i).Sma)
                {
                    var item_1 = lDataQuote.ElementAt(i - 1);
                    if(item_1.Close < (decimal)lBB.ElementAt(i - 1).Sma)
                    {
                        lSig.Add(item);
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);

                if(_check2Sell)
                {
                    var result = Sell(item, lEMA12.ElementAt(i), lEMA12.ElementAt(i), lBB.ElementAt(i));
                    if (!result)
                        continue;

                    //Tính toán các thông số
                    _ItemCheckCur = null;
                    _check2Sell = false;
                    continue;
                }

                var entitySignal = lSig.FirstOrDefault(x => x.Date == item.Date);
                if (entitySignal is null)
                    continue;

                _ItemCheckCur = entitySignal;
                _check2Sell = true;
            }
        }
    }

    public class Info29032024
    {
        public Quote ItemSig { get; set; }
        public Quote ItemSell { get; set; }
        public Quote ItemBot { get; set; }
    }
}

/*
 B1. Tìm tất cả tín hiệu mà Close nến hiện tại > Ma20, nến xanh và nến ngay trước đó có Close < MA20
 */