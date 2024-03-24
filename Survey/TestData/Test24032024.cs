using Skender.Stock.Indicators;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.TestData
{
    public class Test24032024
    {
        public static void MainFunc()
        {
            Console.WriteLine("Coin, Rate Max, Num Max, Rate Min, Num Min");
            Test2("btcusdt");
        }

        public static void print()
        {
            var lTrenMa20 = _lResult.Where(x => x.Loc1 == ELoc1.TrenMa20);
            var lDuoiMa20 = _lResult.Where(x => x.Loc1 == ELoc1.DuoiMa20);
            var lCatMa20Xanh = _lResult.Where(x => x.Loc1 == ELoc1.CatMa20Xanh);
            var lCatMa20Do = _lResult.Where(x => x.Loc1 == ELoc1.CatMa20Do);
            printDetail(lTrenMa20, "Tren MA20");
            printDetail(lDuoiMa20, "Duoi MA20");
            printDetail(lCatMa20Xanh, "Cat MA20 Xanh");
            printDetail(lCatMa20Do, "Cat MA20 Do");
        }

        private static void printDetail(IEnumerable<cls24032024> lInput, string title)
        {
            var total = _lResult.Count();
            var count = lInput.Count();
            var nenXanh = lInput.Count(x => x.TrangThaiNen == ETrangThaiNen.Xanh);
            var nenDo = lInput.Count(x => x.TrangThaiNen == ETrangThaiNen.Do);
            var trungTinh = lInput.Count(x => x.TrangThaiNen == ETrangThaiNen.TrungTinh);
            var msg = $"{title}:\n" +
                $"+Ti Le: {Math.Round((float)count*100/total, 1)}%\n" +
                $"+ Nen Xanh: {Math.Round((float)nenXanh*100/count, 1)}%\n" +
                $"+ Nen Do: {Math.Round((float)nenDo * 100 / count, 1)}%\n" +
                $"+ Trung Tinh: {Math.Round((float)trungTinh * 100 / count, 1)}%";
            Console.WriteLine(msg);
        }

        public static List<cls24032024> _lResult = new List<cls24032024>();
        public static void Test2(string coin)
        {
            var lDataQuote = Data.GetDataAll(coin, EInterval.I1D).Select(x => x.To<Quote>());
            var lEMA5 = lDataQuote.GetEma(5);
            var lEMA12 = lDataQuote.GetEma(12);
            var lRsi = lDataQuote.GetRsi();
            var lBB = lDataQuote.GetBollingerBands();
            var lIchi = lDataQuote.GetIchimoku();
            var count = lDataQuote.Count();

            Quote itemCur = null;
            for (int i = 0; i < count; i++)
            {
                var item = lDataQuote.ElementAt(i);
                var itemEMA5 = lEMA5.ElementAt(i);
                var itemEMA12 = lEMA12.ElementAt(i);
                var itemRsi = lRsi.ElementAt(i);
                var itemBB = lBB.ElementAt(i);
                var itemIchi = lIchi.ElementAt(i);

                if (itemEMA5.Ema is null
                    || itemEMA12.Ema is null
                    || itemRsi.Rsi is null
                    || itemBB.LowerBand is null
                    || itemIchi.SenkouSpanA is null
                )
                    continue;

                if (itemCur == null)
                {
                    itemCur = item;
                    continue;
                }

                var itemUp = item.Open > item.Close ? item.Open : item.Close;
                var itemDown = item.Open < item.Close ? item.Open : item.Close;
                var div = item.Open - item.Close;
                var itemCurUp = itemCur.Open > itemCur.Close ? itemCur.Open : itemCur.Close;
                var itemCurDown = itemCur.Open < itemCur.Close ? itemCur.Open : itemCur.Close;
                var divCur = itemCur.Open - itemCur.Close;

                //Check thân Nến hiện tại > MA20 thì nến kế tiếp xanh hay đỏ

                #region Trạng Thái Nến
                var checkNen = ((item.Close - item.Open) * 100 / item.Open);
                var trangThaiNen = checkNen > 0 ? ETrangThaiNen.Xanh : ETrangThaiNen.Do;
                if (Math.Abs(checkNen) < (decimal)0.5)
                {
                    trangThaiNen = ETrangThaiNen.TrungTinh;
                }
                #endregion

                #region Độ dài thân nến
                EDoDaiThanNen doDaiThanNen;
                var checkNenAbs = Math.Abs(checkNen);
                if (checkNenAbs >= (decimal)11)
                {
                    doDaiThanNen = EDoDaiThanNen.E11;
                }
                else if (checkNenAbs >= (decimal)8)
                {
                    doDaiThanNen = EDoDaiThanNen.E8_11;
                }
                else if (checkNenAbs >= (decimal)5)
                {
                    doDaiThanNen = EDoDaiThanNen.E5_8;
                }
                else if (checkNenAbs >= (decimal)2)
                {
                    doDaiThanNen = EDoDaiThanNen.E2_5;
                }
                else if (checkNenAbs >= (decimal)0.5)
                {
                    doDaiThanNen = EDoDaiThanNen.E05_2;
                }
                else
                {
                    doDaiThanNen = EDoDaiThanNen.E05;
                }
                #endregion

                #region Hình dáng nến
                EHinhDangNen hinhDangNen = EHinhDangNen.None;
                if ((item.High <= itemCur.High && item.Low >= itemCur.Low)
                    || (itemUp <= itemCurUp && itemDown >= itemCurDown))
                {
                    hinhDangNen = EHinhDangNen.InsideBar;
                }
                else
                {
                    var itemThanNen = itemUp - itemDown;
                    var itemRauTren = item.High - itemUp;
                    var itemRauDuoi = itemDown - item.Low;
                    if (itemThanNen * 100 / itemDown < (decimal)0.2)
                    {
                        hinhDangNen = EHinhDangNen.Doji;
                    }
                    else if (itemRauDuoi >= 3 * itemThanNen && itemThanNen >= itemRauTren)
                    {
                        hinhDangNen = EHinhDangNen.Hammer;
                    }
                    else if (itemRauTren >= 3 * itemThanNen && itemThanNen >= itemRauDuoi)
                    {
                        hinhDangNen = EHinhDangNen.HammerInvert;
                    }
                }
                #endregion

                #region Lọc 1
                ELoc1 loc1;
                if (itemCurDown > (decimal)itemBB.Sma)
                {
                    loc1 = ELoc1.TrenMa20;
                }
                else if (itemCurUp < (decimal)itemBB.Sma)
                {
                    loc1 = ELoc1.DuoiMa20;
                }
                else if (divCur > 0)
                {
                    loc1 = ELoc1.CatMa20Xanh;
                }
                else
                {
                    loc1 = ELoc1.CatMa20Do;
                } 
                #endregion

                _lResult.Add(new cls24032024
                {
                    ItemCur = itemCur,
                    ItemNext = item,
                    TrangThaiNen = trangThaiNen,
                    DoDaiThanNen = doDaiThanNen,
                    HinhDangNen = hinhDangNen,
                    Loc1 = loc1
                });
                itemCur = item;
            }
            print();
        }
    }

    public class cls24032024
    {
        public Quote ItemCur { get; set; }
        public Quote ItemNext { get; set; }

        //Trạng thái nến
        public ETrangThaiNen TrangThaiNen { get; set; }  //Nến xanh = 1, Nến đỏ = 2, Nến trung tính(< 0.5) = 3
        //Độ dài thân nến
        public EDoDaiThanNen DoDaiThanNen { get; set; } //Thân nến < 0.5%, 2%, 5%, 8%, 11%
        //Hình dáng nến 
        public EHinhDangNen HinhDangNen { get; set; } //Doji, Hammer, HammerInvert, Inside Bar , Fakey(hơi khó và trùng với các dạng nến phía trước)

        ////////////////////////////////////////////////
        //Lọc 1: Check thân nến hiện tại > MA20 , cắt MA20 xanh, cắt MA20 đỏ, < MA20 (chỉ tính thân nến) thì nến kế tiếp xanh hay đỏ
        public ELoc1 Loc1 { get; set; }
    }

    public enum ETrangThaiNen
    {
        Xanh = 1,
        Do = 2,
        TrungTinh = 3
    }

    public enum EDoDaiThanNen
    {
        E05 = 1,//<0.5
        E05_2 = 2,//0.5>= và < 2
        E2_5 = 3,//>= 2 và < 5
        E5_8 = 4,//>= 5 và < 8
        E8_11 = 5,//>= 8 và < 11
        E11 = 6//>= 11
    }

    public enum EHinhDangNen
    {
        None = 0,
        Doji = 1,
        Hammer = 2,
        HammerInvert = 3,
        InsideBar = 4
    }

    //Lọc 1
    public enum ELoc1
    {
        TrenMa20 = 1,
        CatMa20Xanh = 2,
        CatMa20Do = 3,
        DuoiMa20 = 4
    }
}
