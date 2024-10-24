using MongoDB.Driver.Linq;
using Skender.Stock.Indicators;
using StockLib.DAL.Entity;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace StockLib.Utils
{
    public static class ExtensionMethod
    {
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp, bool isSecond = true)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if(isSecond)
            {
                dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            }
            else
            {
                dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            }
            
            return dateTime;
        }

        public static DateTime UnixTimeStampMinisecondToDateTime(this long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }


        public static DateTime UnixTimeStampToDateTime(this decimal unixTimeStamp, bool isSecond = true)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (isSecond)
            {
                dateTime = dateTime.AddSeconds((double)unixTimeStamp).ToLocalTime();
            }
            else
            {
                dateTime = dateTime.AddMilliseconds((double)unixTimeStamp).ToLocalTime();
            }
            
            return dateTime;
        }

        public static decimal GetRateCandleStick(this Quote val)
        {
            if (val.High == val.Low)
                return 0;
            return Math.Abs(Math.Round(100 * (val.Close - val.Open) / (val.High - val.Low), 1));
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string To2Digit(this int val)
        {
            if (val > 9)
                return val.ToString();
            return $"0{val}";
        }

        public static bool IsTonKho(this Stock stock)
        {
            if(stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.XayDung,
                (int)EStockType.KCN,
                (int)EStockType.BDS,
                (int)EStockType.Thep,
                (int)EStockType.BanLe,
                (int)EStockType.Oto,
                (int)EStockType.OtoTai,
                (int)EStockType.PhanBon,
                (int)EStockType.Than,
                (int)EStockType.XiMang,
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsFDI(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.KCN
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsNguoiMua(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.KCN,
                (int)EStockType.BDS
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsXNK(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Thep,
                (int)EStockType.Than,
                (int)EStockType.ThuySan,
                (int)EStockType.PhanBon,
                (int)EStockType.HoaChat,
                (int)EStockType.Go,
                (int)EStockType.Gao,
                (int)EStockType.XiMang,
                (int)EStockType.CaPhe,
                (int)EStockType.CaoSu,
                (int)EStockType.Oto,
                (int)EStockType.OtoTai,
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsForeign(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.DauKhi
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsBanLe(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.BanLe
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsBDS(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.BDS
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsCangBien(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.CangBien
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsHangKhong(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.HangKhong
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsCaoSu(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.CaoSu
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsCaPhe(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.CaPhe
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsDauTuCong(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.DauTuCong
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsDetMay(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.DetMay
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsGao(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Gao
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsGo(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Go
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsNhua(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Nhua
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsOto(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Oto
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsOtoTai(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.OtoTai
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsPhanBon(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.PhanBon
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsThan(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Than
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsThep(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Thep
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsThuySan(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.Thep
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsXimang(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.XiMang
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsHoaChat(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.HoaChat
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsNganHang(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.NganHang
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsChungKhoan(this Stock stock)
        {
            if (stock.cat is null)
                return false;

            var lCat = new List<int>
            {
                (int)EStockType.ChungKhoan
            };
            foreach (var item in lCat)
            {
                if (stock.cat.Any(x => x.ty == item))
                    return true;
            }

            return false;
        }

        public static bool IsCrude_Oil(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Crude_Oil);
        }

        public static bool IsNatural_gas(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Natural_gas);
        }

        public static bool IsCoal(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Coal);
        }

        public static bool IsGold(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Gold);
        }

        public static bool IsSteel(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Steel);
        }

        public static bool IsHRC_Steel(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.HRC_Steel);
        }

        public static bool IsRubber(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Rubber);
        }

        public static bool IsCoffee(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Coffee);
        }

        public static bool IsRice(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Rice);
        }

        public static bool IsSugar(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Sugar);
        }

        public static bool IsUrea(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Urea);
        }

        public static bool IsMilk(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.milk);
        }

        public static bool IsPolyvinyl(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.polyvinyl);
        }

        public static bool IsNickel(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.Nickel);
        }

        public static bool IsWCI(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.WCI);
        }

        public static bool IsYellowPhotpho(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.YellowPhotpho);
        }

        public static bool IsBDTI(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.BDTI);
        }

        public static bool IsDXY(this Stock stock)
        {
            if (stock.f is null)
                return false;

            return stock.f.Any(x => x == (int)EPrice.DXY);
        }

        public static long GetPrevQuarter(this int time)
        {
            var year = time / 10;
            var quarter = time - year * 10;
            if (quarter == 1)
            {
                year--;
                quarter = 4;
            }
            else
            {
                quarter--;
            }
            return int.Parse($"{year}{quarter}");
        }

        public static long GetYoyQuarter(this int time)
        {
            var year = time / 10;
            var quarter = time - year * 10;
            return int.Parse($"{year - 1}{quarter}");
        }

        public static string GetNameQuarter(this int time)
        {
            var year = time / 10;
            var quarter = time - year * 10;
            return $"{quarter.To2Digit()}/{year - 2000}";
        }

        public static string GetNameHaiQuan(this int time)
        {
            var year = time / 1000;
            var month = (time - year * 1000) / 10;
            var day = time - (year * 1000 + month * 10);
            var mode = day <= 1 ? 1 : 2;
            return $"{month}{mode}-{year - 2000}";
        }

        public static string GetNameMonth(this int time)
        {
            var year = time / 100;
            var month = time - year * 100;
            return $"{month.To2Digit()}/{year - 2000}";
        }

        public static long GetPrevQuarter(this long time)
        {
            var year = time / 10;
            var quarter = time - year * 10;
            if(quarter == 1)
            {
                year--;
                quarter = 4;
            }
            else
            {
                quarter--;
            }
            return long.Parse($"{year}{quarter}");
        }

        public static int GetQuarter(this DateTime dt)
        {
            if (dt.Month <= 3)
                return 1;
            if (dt.Month <= 6)
                return 2;
            if(dt.Month <= 9)
                return 3;
            return 4;
        }

        public static string GetQuarterStr(this DateTime dt)
        {
            if (dt.Month <= 3)
                return "I";
            if (dt.Month <= 6)
                return "II";
            if (dt.Month <= 9)
                return "III";
            return "IV";
        }

        public static DateTime ToDateTime(this string val, string format)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(val))
                    return DateTime.MinValue;
                DateTime dt = DateTime.ParseExact(val, format, CultureInfo.InvariantCulture);
                return dt;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static string RemoveSpace(this string val)
        {
            return val.Replace(" ","").Replace(",","").Replace(".","").Replace("-","").Replace("_","").Trim();
        }

        public static string RemoveSignVietnamese(this string val)
        {
            var VietnameseSigns = new string[]
            {

                "aAeEoOuUiIdDyY",

                "áàạảãâấầậẩẫăắằặẳẵ",

                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

                "éèẹẻẽêếềệểễ",

                "ÉÈẸẺẼÊẾỀỆỂỄ",

                "óòọỏõôốồộổỗơớờợởỡ",

                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

                "úùụủũưứừựửữ",

                "ÚÙỤỦŨƯỨỪỰỬỮ",

                "íìịỉĩ",

                "ÍÌỊỈĨ",

                "đ",

                "Đ",

                "ýỳỵỷỹ",

                "ÝỲỴỶỸ"
            };
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    val = val.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return val;
        }
    }
}
