using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.Util
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
    }
}
