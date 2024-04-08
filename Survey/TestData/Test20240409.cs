using Skender.Stock.Indicators;
using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Survey.TestData
{
    public class Test20240409
    {
        //Hiệu suất/tháng
        private static List<Quote> _lDataQuote = Data.GetDataAll("btcusdt", EInterval.I1D).Select(x => x.To<Quote>()).ToList();
        private static int _count = 0;
        private const decimal _minWidthCandle = (decimal)0.5;//nếu thân nến < 0.5% thì không tính
        public static void MainFunc()
        {
            foreach (var item in StaticVal._lSymbol)
            {
                _lDataQuote = Data.GetDataAll(item.Symbol, EInterval.I1D).Select(x => x.To<Quote>()).ToList();
                _count = _lDataQuote.Count();
                Check2Buy();
                Analyze();
                Mapping();
                print(item.Symbol);
                Thread.Sleep(500);
            }
            printTotal();
        }
        private static List<(Quote, Quote)> _lSig = new List<(Quote, Quote)>();
        private static void Check2Buy()
        {
            var flagCheck = false;
            var countCheck = -1;
            Quote quoteBot = null;
            Quote quoteCheck = null;
            for (int i = 1; i < _count; i++)
            {
                if (flagCheck)
                {
                    if (countCheck > 5)
                    {
                        quoteCheck = null;
                        flagCheck = false;
                        countCheck = -1;
                        continue;
                    }
                    countCheck++;
                }

                var item = _lDataQuote.ElementAt(i);
                //if (item.Date.Year == 2017 && item.Date.Month == 12 && item.Date.Day == 24)
                //{
                //    var tmp = 1;
                //}
                if (item.Close > item.Open
                       && !flagCheck)
                {
                    var item_1 = _lDataQuote.ElementAt(i - 1);
                    if (item_1.Close < item_1.Open
                        && item_1.Open < item.Close
                        && Math.Round(Math.Abs(item_1.Open - item_1.Close) * 100 / item_1.Close, 2) > _minWidthCandle)
                    {
                        quoteCheck = FindRedCandle(i - 1, item_1);
                        if (quoteCheck is null)
                        {
                            //không tìm được nến check thì reset lại
                            continue;
                        }

                        flagCheck = true;
                        quoteBot = item_1;
                    }
                }

                if (flagCheck
                    && item.Close > quoteCheck.Open)
                {
                    _lSig.Add((item, quoteBot));
                    quoteBot = null;
                    quoteCheck = null;
                    flagCheck = false;
                    countCheck = -1;
                }
            }
        }

        private static Quote FindRedCandle(int index, Quote quote)
        {
            if (index < 6)
                return null;
            for (int i = index - 1; i > index - 6; i--)
            {
                var item = _lDataQuote.ElementAt(i);
                if (item.Open > item.Close
                    && item.Open > quote.Open)
                {
                    var rate = Math.Round(Math.Abs(item.Open - item.Close) * 100 / item.Close, 2);
                    if (rate < _minWidthCandle)
                        continue;
                    return item;
                }
            }
            return null;
        }

        public static List<Info20240409> _lResult = new List<Info20240409>();
        private static (Quote, Quote) _ItemCheckCur;
        private static bool _check2Sell = false;
        private static bool _hasAboveMa20 = false;
        private static bool Check2Sell(Quote item, SmaResult ma)
        {
            if (!_check2Sell)
                return false;

            //CUTLOSS
            if (item.Close < _ItemCheckCur.Item1.Open)
                return true;

            if (ma.Sma is null)
                return false;

            if (item.Close >= (decimal)ma.Sma)
            {
                _hasAboveMa20 = true;
            }

            if (_hasAboveMa20
                && item.Close < (decimal)ma.Sma)
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

                    var objPrint = new Info20240409
                    {
                        NenMua = _ItemCheckCur.Item1,
                        NenDay = _ItemCheckCur.Item2,
                        NenBan = item,
                    };
                    _lResult.Add(objPrint);

                    //reset
                    _ItemCheckCur.Item1 = null;
                    _ItemCheckCur.Item2 = null;
                    _check2Sell = false;
                    continue;
                }

                var entitySignal = _lSig.FirstOrDefault(x => x.Item1.Date == item.Date);
                if (entitySignal.Item1 is null)
                    continue;

                _ItemCheckCur = entitySignal;
                _check2Sell = true;
                _hasAboveMa20 = false;
            }
        }

        private static List<Info20240409VM> _lMapping = new List<Info20240409VM>();
        public static void Mapping()
        {
            foreach (var item in _lResult)
            {
                _lMapping.Add(new Info20240409VM
                {
                    NgayMua = item.NenMua.Date,
                    NgayBan = item.NenBan.Date,
                    SoNenNamGiu = (item.NenBan.Date - item.NenMua.Date).TotalDays,
                    Tile = Math.Round((item.NenBan.Close - item.NenMua.Close) * 100 / item.NenMua.Close, 2),
                    KCTuDay = Math.Round((item.NenMua.Close - item.NenDay.Close) * 100 / item.NenDay.Close, 2),
                    Goc = TinhGoc(item.NenMua, item.NenDay)
                });
            }
        }

        public static int TinhGoc(Quote itemCur, Quote itemBot)
        {
            var low = Math.Min(itemBot.Close, itemBot.Open);
            decimal divVal = 100 * (itemCur.Close - low) / low;
            var div = (itemCur.Date - itemBot.Date).TotalDays;
            var res = div / Math.Sqrt((double)(divVal * divVal + (decimal)(div * div)));

            if (res > 0.985)
                return 10;
            if (res > 0.94)
                return 20;
            if (res > 0.866)
                return 30;
            if (res > 0.766)
                return 40;
            if (res > 0.64)
                return 50;
            if (res > 0.5)
                return 60;
            if (res > 0.34)
                return 70;
            if (res > 0.17)
                return 80;
            return 90;
        }

        private static List<Info20240409VM> _lTotal = new List<Info20240409VM>();
        public static void print(string symbol)
        {
            foreach (var item in _lMapping)
            {
                item.Symbol = StaticVal._lSymbol.First(x => x.Symbol == symbol);
            }
            _lTotal.AddRange(_lMapping.ToList());

            //var tmp = _lMapping.Count(x => x.Tile >= 0);
            //var tmp1 = _lMapping.Count(x => x.Tile >= 10);
            //Console.WriteLine($"{symbol}|So nen: { _lMapping.Sum(x => x.SoNenNamGiu)}| Ti le: {_lMapping.Sum(x => x.Tile)}");
            _lResult.Clear();
            _lMapping.Clear();
            _lSig.Clear();
            _ItemCheckCur.Item1 = null;
            _ItemCheckCur.Item2 = null;
            _check2Sell = false;
            _hasAboveMa20 = false;
            //var tmp2 = _lMapping.Sum(x => x.Tile);
            //var tmp3 = _lMapping.Sum(x => x.SoNenNamGiu);
            //Console.WriteLine("Ngay Mua, Ngay Ban, So Nen, Goc, KC, Take Profit(%)");
            //foreach (var item in _lMapping)
            //{
            //    Console.WriteLine($"{item.NgayMua.ToString("dd/MM/yyyy")}," +
            //                    $"{item.NgayBan.ToString("dd/MM/yyyy")}," +
            //                    $"{item.SoNenNamGiu}," +
            //                    $"{item.Goc}," +
            //                    $"{item.KCTuDay}%," +
            //                    $"{item.Tile}%");
            //}
        }

        public static void printTotal()
        {
            _lTotal = _lTotal.OrderBy(x => x.NgayMua.Date).ThenBy(x => x.Symbol.Priority).ToList();
            var lCheck = new List<Info20240409VM>();
            DateTime dtCheck = DateTime.MinValue;
            foreach (var item in _lTotal)
            {
                if(item.NgayMua <= dtCheck)
                {
                    continue;
                }
                dtCheck = item.NgayBan;

                lCheck.Add(item);
            }

            Console.WriteLine($"Tile: {lCheck.Sum(x => x.Tile)}");
            Console.WriteLine($"SoNen: {lCheck.Sum(x => x.SoNenNamGiu)}");
            foreach (var item in lCheck)
            {
                Console.WriteLine($"{item.Symbol.Symbol}" +
                                $"{item.NgayMua.ToString("dd/MM/yyyy")}," +
                                $"{item.NgayBan.ToString("dd/MM/yyyy")}," +
                                $"{item.SoNenNamGiu}," +
                                $"{item.Goc}," +
                                $"{item.KCTuDay}%," +
                                $"{item.Tile}%");
            }
        }
    }

    public class Info20240409
    {
        public Quote NenDay { get; set; }
        public Quote NenMua { get; set; }
        public Quote NenBan { get; set; }
    }

    public class Info20240409VM
    {
        public SymbolCustom Symbol { get; set; }
        public DateTime NgayMua { get; set; }
        public DateTime NgayBan { get; set; }
        public double SoNenNamGiu { get; set; }
        public double Goc { get; set; }
        public decimal KCTuDay { get; set; }
        public decimal Tile { get; set; }
    }
}
