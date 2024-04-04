using Skender.Stock.Indicators;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Survey.TestData
{
    public class Test20240404
    {
        private static List<Quote> _lDataQuote = Data.GetDataAll("btcusdt", EInterval.I1D).Select(x => x.To<Quote>()).ToList();
        private static int _count = 0;
        public static void MainFunc()
        {
            _count = _lDataQuote.Count();
            Check2Buy();
            Analyze();
            print();
        }
        private static List<Quote> _lSig = new List<Quote>();
        private static void Check2Buy()
        {
            var flagCheck = false;
            var countCheck = -1;
            Quote quoteCheck = null;
            for (int i = 1; i < _count; i++)
            {
                if(flagCheck)
                {
                    if(countCheck > 5)
                    {
                        quoteCheck = null;
                        flagCheck = false;
                        countCheck = -1;
                        continue;
                    }
                    countCheck++;
                }

                var item = _lDataQuote.ElementAt(i);
                if (item.Close > item.Open
                       && !flagCheck)
                {
                    var item_1 = _lDataQuote.ElementAt(i - 1);
                    if (item_1.Close < item_1.Open
                        && item_1.Open < item.Close)
                    {
                        quoteCheck = FindRedCandle(i - 1, item_1);
                        if (quoteCheck is null)
                        {
                            //không tìm được nến check thì reset lại
                            continue;
                        }

                        flagCheck = true;
                    }
                }

                if (flagCheck
                    && item.Close > quoteCheck.Open)
                {
                    quoteCheck = null;
                    flagCheck = false;
                    countCheck = -1;
                    _lSig.Add(item);
                }
            }
        }

        private static Quote FindRedCandle(int index, Quote quote)
        {
            for (int i = index - 1; i > index - 6; i--)
            {
                var item = _lDataQuote.ElementAt(i);
                if (item.Open > item.Close
                    && item.Open > quote.Open)
                {
                    var rate = Math.Round(Math.Abs(item.Open - item.Close) * 100 / item.Close, 2);
                    if (rate < (decimal)0.5) //nếu thân nến < 0.5% thì không tính
                        continue;
                    return item;
                }
            }
            return null;
        }

        public static List<Info20240404> _lResult = new List<Info20240404>();
        private static Quote _ItemCheckCur = null;
        private static bool _check2Sell = false;
        private static bool _hasAboveMa20 = false;
        private static bool Check2Sell(Quote item, SmaResult ma)
        {
            if (!_check2Sell)
                return false;

            if(item.Close >= (decimal)ma.Sma)
            {
                _hasAboveMa20 = true;
            }

            if (_hasAboveMa20 
                && item.Close < (decimal)ma.Sma)
                return true;

            //CUTLOSS
            if (item.Close < _ItemCheckCur.Open)
                return true;

            return false;
        }
        public static void Analyze()
        {
            var lSma20 = _lDataQuote.GetSma(20);
            for (int i = 0; i < _count; i++)
            {
                var item = _lDataQuote.ElementAt(i);
                if (_check2Sell)
                {
                    var result = Check2Sell(item, lSma20.ElementAt(i));
                    if (!result)
                        continue;

                    var objPrint = new Info20240404
                    {
                        NgayMua = _ItemCheckCur.Date,
                        NgayBan = item.Date,
                        SoNenNamGiu = (item.Date - _ItemCheckCur.Date).TotalDays,
                        Tile = Math.Round((item.Close - _ItemCheckCur.Close) * 100 / _ItemCheckCur.Close, 2)
                    };
                    _lResult.Add(objPrint);

                    //reset
                    _ItemCheckCur = null;
                    _check2Sell = false;
                    continue;
                }

                var entitySignal = _lSig.FirstOrDefault(x => x.Date == item.Date);
                if (entitySignal is null)
                    continue;

                _ItemCheckCur = entitySignal;
                _check2Sell = true;
            }
        }
        public static void print()
        {
            var tmp = _lResult.Count(x => x.Tile >= 0);
            var tmp1 = _lResult.Count(x => x.Tile >= 10);
            Console.WriteLine("Ngay Mua, Ngay Ban, So Nen, Take Profit(%)");
            foreach (var item in _lResult)
            {
                Console.WriteLine($"{item.NgayMua.ToString("dd/MM/yyyy")}," +
                                $"{item.NgayBan.ToString("dd/MM/yyyy")}," +
                                $"{item.SoNenNamGiu}," +
                                $"{item.Tile}");
            }
        }
    }

    public class Info20240404
    {
        public DateTime NgayMua { get; set; }
        public DateTime NgayBan { get; set; }
        public double SoNenNamGiu { get; set; }
        public decimal Tile { get; set; }
    }
}
