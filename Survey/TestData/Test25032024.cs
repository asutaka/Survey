using Skender.Stock.Indicators;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.TestData
{
    public class Test25032024
    {
        public static void MainFunc()
        {
            Console.WriteLine("Coin, Rate Max, Num Max, Rate Min, Num Min");
            Test2("btcusdt");
            print();
        }

        public static void print()
        {
            foreach (var item in _lResult)
            {
                Console.WriteLine($"Ngay: {item.Ngay.ToString("dd/MM/yyyy")}\t" +
                                $"Index UP: {item.IndexUp}; Rate UP: {item.RateUp}%\t" +
                                $"Index DOWN: {item.IndexDown}; Rate DOWN: {item.RateDown}%");
            }
        }

        public static List<cls25032024> _lResult = new List<cls25032024>();
        public static void Test2(string coin)
        {
            var lDataQuote = Data.GetDataAll(coin, EInterval.I1D).Select(x => x.To<Quote>());
            var lEMA5 = lDataQuote.GetEma(5);
            var lEMA12 = lDataQuote.GetEma(12);
            var lRsi = lDataQuote.GetRsi();
            var lBB = lDataQuote.GetBollingerBands();
            var lIchi = lDataQuote.GetIchimoku();
            var lMACD = lDataQuote.GetMacd();
            var count = lDataQuote.Count();

            Quote itemCur = null;
            RsiResult itemRsi = null;
            EmaResult itemEma5 = null, itemEma12 = null;
            BollingerBandsResult itemBB = null;
            MacdResult itemMACD = null;
            IchimokuResult itemIchi = null;

            List<Quote> lSig = new List<Quote>();
            bool flag = false;
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);

                if (lEMA5.ElementAt(i).Ema is null
                    || lEMA12.ElementAt(i).Ema is null
                    //|| lRsi.ElementAt(i).Rsi is null
                    //|| lBB.ElementAt(i).LowerBand is null
                    //|| lIchi.ElementAt(i).SenkouSpanA is null
                    //|| lMACD.ElementAt(i).Histogram is null
                )
                    continue;

                if(!flag)
                {
                    if(lEMA5.ElementAt(i).Ema >= lEMA12.ElementAt(i).Ema)
                    {
                        flag = true;
                        lSig.Add(item);
                    }
                }
                else if(lEMA5.ElementAt(i).Ema < lEMA12.ElementAt(i).Ema)
                {
                    flag = false;
                }
            }

            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);
                var checkSignal = lSig.Any(x => x.Date == item.Date);
                if (checkSignal)
                {
                    var lTake7 = lDataQuote.Skip(i).Take(7);
                    decimal high = 0, low = 0;
                    int indexHigh = 0, indexLow = 0;
                    var countTake7 = lTake7.Count();
                    for (int j = 0; j < countTake7; j++)
                    {
                        var itemTake = lTake7.ElementAt(j);
                        var itemTakeUp = itemTake.Close >= itemTake.Open ? itemTake.Close : itemTake.Open;
                        var itemTakeDown = itemTake.Close < itemTake.Open ? itemTake.Close : itemTake.Open;

                        if (itemTakeUp > high)
                        {
                            high = itemTakeUp;
                            indexHigh = j;
                        }

                        if(itemTakeDown < low || low == 0)
                        {
                            low = itemTakeDown;
                            indexLow = j;
                        }
                    }

                    var itemUp = item.Close >= item.Open ? item.Close : item.Open;
                    var itemDown = item.Close < item.Open ? item.Close : item.Open;
                    var rateUp = Math.Round((high - itemUp) * 100 / itemUp, 1);
                    var rateDown = Math.Round((low - itemDown) * 100 / itemDown, 1);
                    _lResult.Add(new cls25032024
                    {
                        Ngay = item.Date,
                        RateUp = rateUp,
                        RateDown = rateDown,
                        IndexUp = indexHigh,
                        IndexDown = indexLow
                    });

                    //var lTake11 = lDataQuote.Skip(i).Take(11);
                }
            }
        }
    }

    public class cls25032024
    {
        public DateTime Ngay { get; set; }
        public decimal RateUp { get; set; }
        public decimal RateDown { get; set; }
        public int IndexUp { get; set; }
        public int IndexDown { get; set; }
    }
}
