using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Utils
{
    public static class ExtensionMethod
    {
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
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

        public static string FormatVietnamese(this string val)
        {
            var res = val.Replace("ð", "Ự")
                        .Replace("«", "ẫ")
                        .Replace("Ñ", "ố")
                        .Replace("Ë", "ị")
                        .Replace("¡", "ạ")
                        .Replace("\u0003", "ă")
                        .Replace("\u0010", "Đ")
                        .Replace("¢", "Ả")
                        .Replace("£", "ả")
                        .Replace("¯", "ắ")
                        .Replace("ñ", "ự")
                        .Replace("¿", "ế")
                        .Replace("ç", "ả")
                        .Replace("§", "ầ")
                        .Replace("ï", "ữ")
                        .Replace("\u0011", "đ")
                        .Replace("®", "Ắ")
                        .Replace("Û", "ớ")
                        .Replace("¥", "ấ")
                        .Replace("ë", "ừ")
                        .Replace("ß", "ở")
                        .Replace("ãc", "ợc")
                        .Replace("iÁn", "iền")
                        .Replace("hÉ", "hỉ")
                        .Replace("Én", "ỉn")
                        .Replace("ảa", "ủa")
                        .Replace("ĐÓI", "ĐỐI")
                        .Replace("KÉ", "KẾ")
                        .Replace("Đạn", "Đơn")
                        .Replace("đÙ", "độ")
                        .Replace("ãp", "ợp")
                        .Replace("Ón", "ồn")
                        .Replace("Ýn", "ờn")
                        .Replace(" H N ", " HẠN ")
                        .Replace("ing", "ũng")
                        .Replace("BÙ", "Bộ")
                        .Replace("HâP", "HỢP")
                        .Replace("ưạn", "ươn")
                        .Replace("éng", "ứng")
                        .Replace("Ý", "ờ")
                        .Replace("Ù", "ộ")
                        .Replace("xí", "xử")
                        .Replace("vÁ", "về")
                        .Replace("hả ", "hủ ")
                        .Replace("iy", "ũy")
                        .Replace("å", "ụ")
                        .Replace("v­t", "vật")
                        .Replace("Ø", "Ộ")
                        .Replace("Nâ", "Nợ")
                        .Replace("Nã", "Nợ")
                        .Replace("nã", "nợ")
                        .Replace("Ç", "ệ")
                        .Replace("Õ", "ổ")
                        .Replace("uù", "ũy")
                        .Replace("×", "ổ")
                        .Replace("Án", "ển")
                        .Replace("Ã", "ể")
                        .Replace("Íc", "ọc")
                        .Replace("æ", "Ủ")
                        .Replace("Þ", "Ở")
                        .Replace("î", "Ữ")
                        .Replace("·", "ặ")
                        .Replace("Ín", "ọn")
                        .Replace("Vôn", "Vốn")
                        .Replace("÷", "ỷ")
                        .Replace("rã", "rợ")
                        .Replace("kó", "kỳ")
                        .Replace("Lãi", "Lợi")
                        .Replace("lãi­", "lợi")
                        .Replace("nhu­n", "nhuận")
                        .Replace("Î", "Ổ")
                        .Replace("Î", "Ổ")
                        .Replace("ÒÒ", "Ồ")
                        .Replace("Å", "ễ")
                        .Replace("ưạ", "ươ")
                        .Replace("l­p", "lập")
                        .Replace("L­p", "Lập")
                        .Replace("HO T", "HOẠT")
                        .Replace("\u0002", "Ă")
                        .Replace("Uờ", "ÚY")
                        .Replace("gp", "gộp")
                        .Replace("cạ", "cơ")
                        .Replace("nh­p", "nhập")
                        .Replace("Lợi nhu­ ", "Lợi nhuận ")
                        .Replace("Kù­", "Kỹ")
                        .Replace("°", "ư");

            var res2 = res.Replace("ưạ", "ươ")
                        .Replace("Tiên", "Tiền")
                        .Replace("hÓi", "thu hồi")
                        .Replace("ÆP", "ỆP")
                        .Replace("Kò", "KỲ")
                        .Replace("Kó", "Kỳ")
                        .Replace("UẢN", "UẨN")
                        .Replace("CHÉ", "CHẾ")
                        .Replace("IÀN", "IỀN")
                        .Replace("Æ", "Ệ")
                        .Replace("Sì", "SỬ")
                        .Replace("äN", "ỤN")
                        .Replace("©", "ẩ")
                        .Replace("t­p", "tập")
                        .Replace("bÏ", "bỏ")
                        .Replace("nh­n", "nhận")
                        .Replace("±", "ằ")
                        .Replace("»", "ể")
                        .Replace("éc", "ức")
                        .Replace("đạn", "đơn")
                        .Replace("íi", "ửi")
                        .Replace("Óm", "ồm")
                        .Replace("ưã", "ượ")
                        .Replace(")a", "ĩa")
                        .Replace("BÒ", "BỔ")
                        .Replace("ä", "Ụ")
                        .Replace("ÁẮ", "Ả")
                        .Replace("sí", "sử")
                        .Replace("Thu­t", "Thuật")
                        .Replace("Õn­", "ổn")
                        .Replace("¥­", "ấ")
                        .Replace("Ë­", "ị")
                        .Replace("vå", "vụ")
                        .Replace("cía", "cửa")
                        .Replace("TÓNG", "TỔNG")
                        .Replace("VÓN", "VỐN")
                        .Replace("iÁ", "iề");
            return res2;
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
