using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOREIGNBUYSELL
{
    public static class ExtensionMethod
    {
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

        public static DateTime ToDateTime(this string val, string format)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(val))
                    return DateTime.MinValue;
                DateTime dt = DateTime.ParseExact(val, format, CultureInfo.InvariantCulture);
                return dt;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }
    }
}
