using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SLib.Util
{
    public static class ExtensionMethod
    {
        public static DateTime UnixTimeStampToDateTime(this decimal unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds((double)unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
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

        public static List<int> AllIndexesOf(this string str, string value)
        {
            var indexes = new List<int>();
            if (string.IsNullOrWhiteSpace(value))
                return indexes;
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
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

        public static string ToQuarter(this int val)
        {
            if (val == 0)
                return "I";
            if (val == 1)
                return "II";
            if (val == 2)
                return "III";
            return "IV";
        }

        public static int GetQuarter(this DateTime date)
        {
            if (date.Month >= 4 && date.Month <= 6)
                return 2;
            else if (date.Month >= 7 && date.Month <= 9)
                return 3;
            else if (date.Month >= 10 && date.Month <= 12)
                return 4;
            else
                return 1;
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
                        //.Replace("ưạ", "ươ")
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
                        //.Replace("tưạng", "tương")
                        .Replace("°", "ư");
            return res;
        }
    }
}
