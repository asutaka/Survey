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
            Test2("btcusdt");
            print();
        }

        public static void print()
        {
            var lPrint = _lResult.Select(x => new Info29032024VM { 
                NgayMua = x.Item_Sig.Date.ToString("dd/MM/yyyy"),
                NgayBan = x.Item_Sell.Date.ToString("dd/MM/yyyy"),
                SoNenNamGiu = (x.Item_Sell.Date - x.Item_Sig.Date).TotalDays,
                TakeProfit = Math.Round((x.Item_Sell.Close - x.Item_Sig.Close) * 100 / x.Item_Sig.Close, 2),
                DoDaiThanNen = Math.Round((x.Item_Sig.Close - x.Item_Sig.Open) * 100 / x.Item_Sig.Open, 1),
                DoDaiRauNen = (Math.Max((x.Item_Sig.High - Math.Max(x.Item_Sig.Open, x.Item_Sig.Close)), (Math.Min(x.Item_Sig.Open, x.Item_Sig.Close) - x.Item_Sig.Low)) * 100 / (x.Item_Sig.High - x.Item_Sig.Low)),
                //GiaMoCuaNhoHonMa20 = x.HasOpenLessMA20 ? "CO" : "KHONG",
                KCTuGiaDongCuaDenMa20 = x.KCTuCloseDenMa20,
                DoRongBB = x.BBWidth_Sig??0,
                //Ema512 = (x.HasEMA5_12 ? "CO" : "KHONG"),
                GocDayTinHieu = x.GocDay_TinHieu,
                KCTuDayDenMa20 = x.KCTuDayDenMa20,
                DoDaiNenBan = Math.Round((x.Item_Sell.Close - x.Item_Sell.Open) * 100 / x.Item_Sell.Open, 1),
                DayNgay = x.Item_Bot.Date.ToString("dd/MM/yyyy"),
                SoNenTuDayDenTinHieu = (x.Item_Sig.Date - x.Item_Bot.Date).TotalDays
            });

            //var tmp1 = lPrint.Count(x => x.TakeProfit > 0);
            //lPrint = lPrint.Where(x => x.GocDayTinHieu >= 50
            //                        && x.SoNenTuDayDenTinHieu <= 5
            //                        && x.KCTuDayDenMa20 < 10
            //                        && x.DoRongBB < 20
            //                        && x.KCTuGiaDongCuaDenMa20 < 5
            //                        && x.DoDaiThanNen < 6
            //                    ).ToList();

            //var tmp1 = lPrint.Sum(x => x.TakeProfit);
            //var tmp2 = 1;
            //lPrint = lPrint.Where(x => x.GocDayTinHieu <= 50).ToList();
            //var tmp21 = lPrint.Sum(x => x.TakeProfit);
            //var tmp2 = 1;
            Console.WriteLine("Ngay Mua, Ngay Ban, So Nen, Take Profit(%), , Do Dai Nen(%), Do Dai Rau Nen(%), KC Tu Close Den Ma20(%), Do Rong BB(%), , Goc Tang, KC Day Den Ma20(%), Do Dai Nen Ban(%), Day Ngay, So Nen Tu Day");
            foreach (var item in lPrint)
            {
                Console.WriteLine($"{item.NgayMua}," +
                                $"{item.NgayBan}," +
                                $"{item.SoNenNamGiu}," +
                                $"{item.TakeProfit}," +
                                $"," +
                                $"{item.DoDaiThanNen}," +
                                $"{item.DoDaiRauNen}," +
                                //$"{item.GiaMoCuaNhoHonMa20}," +
                                $"{item.KCTuGiaDongCuaDenMa20}," +
                                $"{item.DoRongBB}," +
                                $"," +
                                //$"{item.Ema512}," +
                                $"{item.GocDayTinHieu}," +
                                $"{item.KCTuDayDenMa20}," +
                                $"{item.DoDaiNenBan}," +
                                $"{item.DayNgay}," +
                                $"{item.SoNenTuDayDenTinHieu}"); 
            }

            //var tmp = _lResult.Sum(x => Math.Round((x.Item_Sell.Close - x.Item_Sig.Close) * 100 / x.Item_Sig.Close, 2));
            //Console.WriteLine(tmp);
            //_lResult = _lResult.Where(x => Math.Round((x.Item_Sig.Close - x.Item_Sig.Open) * 100 / x.Item_Sig.Open, 1) > 10).ToList();
            //_lResult = _lResult.Where(x => x.BBWidth_Sig >= 30 && x.BBWidth_Sig < 40).ToList();
            //foreach (var item in _lResult)
            //{
            //    var itemSig = item.Item_Sig;
            //    var itemSell = item.Item_Sell;

            //    Console.WriteLine($"{itemSig.Date.ToString("dd/MM/yyyy")}," + //Ngày mua 
            //                        $"{itemSell.Date.ToString("dd/MM/yyyy")}," +//Ngày bán
            //                        $"{(itemSell.Date - itemSig.Date).TotalDays}," +//Số nến nắm giữ
            //                        $"{Math.Round((itemSell.Close - itemSig.Close) * 100 / itemSig.Close, 2)}," + //Take Profit
            //                        $"," +
            //                        $"{Math.Round((itemSig.Close - itemSig.Open) * 100 / itemSig.Open, 1)}," +//Độ dài thân nến
            //                        $"{(Math.Max((itemSig.High - Math.Max(itemSig.Open, itemSig.Close)), (Math.Min(itemSig.Open, itemSig.Close) - itemSig.Low)) * 100 / (itemSig.High - itemSig.Low))}," +//Độ dài râu nến
            //                        $"{(item.HasOpenLessMA20 ? "CO" : "KHONG")}," +//Giá mở cửa < Ma20
            //                        $"{item.KCTuCloseDenMa20}," +//Khoảng cách từ giá đóng cửa đến Ma20
            //                        $"{item.BBWidth_Sig}," +//Độ rộng dải BB
            //                        $"," +
            //                        $"{(item.HasEMA5_12 ? "CO" : "KHONG")}," +//EMA5 cắt trên EMA12
            //                        $"{item.GocDay_TinHieu}," +//Góc từ đáy lên tín hiệu
            //                        $"{item.KCTuDayDenMa20}," +//Khoảng cách từ đáy đến Ma20
            //                        $"{Math.Round((itemSell.Close - itemSell.Open) * 100 / itemSell.Open, 1)}," +//Độ dài nến bán
            //                        $"{item.Item_Bot.Date.ToString("dd/MM/yyyy")}"//Đáy ngày
            //                   );
            //}
        }

        public static List<Info29032024> _lResult = new List<Info29032024>();
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

            ////CASE 1: 
            //if (ema5.Ema < ema12.Ema)
            //    return true;

            //CASE 2: 
            if (item.Close < (decimal)bb.Sma)
                return true;

            ////CASE 3: 
            //if (ema5.Ema < ema12.Ema
            //    && (item.Close < (decimal)bb.Sma))
            //    return true;

            //CUTLOSS
            if (item.Close < _ItemCheckCur.Open)
                return true;

            return false;
        }
        public static void Test2(string coin)
        {
            var lDataQuote = Data.GetDataAll(coin, EInterval.I1D).Select(x => x.To<Quote>()).ToList();
            var lEMA5 = lDataQuote.GetEma(5);
            var lEMA12 = lDataQuote.GetEma(12);
            var lBB = lDataQuote.GetBollingerBands();
            var count = lDataQuote.Count();

            List<Quote> lSig = new List<Quote>();
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);

                if (lEMA5.ElementAt(i).Ema is null
                    || lEMA12.ElementAt(i).Ema is null
                    || lBB.ElementAt(i).LowerBand is null
                )
                    continue;

                if(item.Close > item.Open
                    && item.Close > (decimal)lBB.ElementAt(i).Sma)
                {
                    var item_1 = lDataQuote.ElementAt(i - 1);
                    if(lBB.ElementAt(i - 1).Sma != null 
                        && item_1.Close < (decimal)lBB.ElementAt(i - 1).Sma
                        //&& item_1.Open < (decimal)lBB.ElementAt(i - 1).Sma
                        )
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
                    var result = Sell(item, lEMA5.ElementAt(i), lEMA12.ElementAt(i), lBB.ElementAt(i));
                    if (!result)
                        continue;

                    //if(_ItemCheckCur.Date.Day == 10 && _ItemCheckCur.Date.Month == 1)
                    //{
                    //    var tmp = 1;
                    //}

                    //Tính toán các thông số
                    var indexCheckCur = lDataQuote.ToList().IndexOf(_ItemCheckCur);
                    //var tmp1 = lBB.ElementAt(indexCheckCur).Sma;
                    var itemBot = GetBottomValue(lDataQuote, _ItemCheckCur);
                    var indexBot = lDataQuote.ToList().IndexOf(itemBot);
                    var objPrint = new Info29032024
                    {
                        Item_Sig = _ItemCheckCur,
                        Item_Sell = item,
                        Item_Bot = itemBot,
                        HasEMA5_12 = lEMA5.ElementAt(indexCheckCur).Ema >= lEMA12.ElementAt(indexCheckCur).Ema,
                        HasOpenLessMA20 = _ItemCheckCur.Open < (decimal)lBB.ElementAt(indexCheckCur).Sma,
                        BBWidth_Sig = 100 * (lBB.ElementAt(indexCheckCur).UpperBand - lBB.ElementAt(indexCheckCur).LowerBand) / lBB.ElementAt(indexCheckCur).LowerBand,
                        SoNenTuDayDenTinHieu = indexCheckCur - indexBot,
                        KCTuDayDenMa20 = Math.Round(((decimal)lBB.ElementAt(indexCheckCur).Sma - Math.Min(itemBot.Open, itemBot.Close)) * 100 / Math.Min(itemBot.Open, itemBot.Close), 2),
                        KCTuCloseDenMa20 = Math.Round((_ItemCheckCur.Close - (decimal)lBB.ElementAt(indexCheckCur).Sma) * 100 / (decimal)lBB.ElementAt(indexCheckCur).Sma, 2),
                        GocDay_TinHieu = TinhGoc(_ItemCheckCur, itemBot, indexCheckCur - indexBot)
                    };
                    _lResult.Add(objPrint);

                    //reset
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

        public static Quote GetBottomValue(List<Quote> lInput, Quote itemCur)
        {
            var index = lInput.IndexOf(itemCur);
            var bot = itemCur.Close;
            var itemBot = itemCur;
            for (int i = index - 10; i < index; i++)
            {
                var item = lInput.ElementAt(i);
                var low = Math.Min(item.Open, item.Close);
                if (low < bot)
                {
                    bot = low;
                    itemBot = item;
                }
            }
            return itemBot;
        }
        public static int TinhGoc(Quote itemCur, Quote itemBot, int div)
        {
            var low = Math.Min(itemBot.Close, itemBot.Open);
            decimal divVal = 100 * (itemCur.Close - low) / low;
            var res = div / Math.Sqrt((double)(divVal * divVal + div * div));

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

    }

    public class Info29032024
    {
        public Quote Item_Sig { get; set; }
        public Quote Item_Sell { get; set; }
        public Quote Item_Bot { get; set; }
        //
        public bool HasEMA5_12 { get; set; }//EMA 5 cắt lên EMA 12
        public bool HasOpenLessMA20 { get; set; }//Nến tín hiệu có giá Open < Ma20
        public double? BBWidth_Sig { get; set; }//Độ rộng BB tại tín hiệu
        public int SoNenTuDayDenTinHieu { get; set; }//Số nến từ đáy đến tín hiệu
        public decimal KCTuDayDenMa20 { get; set; }//Khoảng cách giữa giá thấp nhất của thân nến đáy đến MA20
        public decimal KCTuCloseDenMa20 { get; set; }//Khoảng cách giữa giá Close của nến tín hiệu đến MA20
        public int GocDay_TinHieu { get; set; }//Góc giữa giá thấp nhất của thân nến đáy và giá thấp nhất của thân nến tín hiệu
    }

    public class Info29032024VM
    {
        public string NgayMua { get; set; }
        public string NgayBan { get; set; }
        public double SoNenNamGiu { get; set; }
        public decimal TakeProfit { get; set; }
        public decimal DoDaiThanNen { get; set; }
        public decimal DoDaiRauNen { get; set; }
        public string GiaMoCuaNhoHonMa20 { get; set; }
        public decimal KCTuGiaDongCuaDenMa20 { get; set; }
        public double DoRongBB { get; set; }
        public string Ema512 { get; set; }
        public decimal GocDayTinHieu { get; set; }
        public decimal KCTuDayDenMa20 { get; set; }
        public decimal DoDaiNenBan { get; set; }
        public string DayNgay { get; set; }
        public double SoNenTuDayDenTinHieu { get; set; }
    }
}

/*
 B1. Tìm tất cả tín hiệu mà Close nến hiện tại > Ma20, nến xanh và nến ngay trước đó có Close < MA20
 */