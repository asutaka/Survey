using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class GDNNAPIModel
    {
        public GDNNAPI_Data data { get; set; }
        public int status { get; set; }
    }

    public class GDNNAPI_Data
    {
        public List<GDNNAPI_DataDetail> data { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
    }

    public class GDNNAPI_DataDetail
    {
        public string symbol { get; set; }
        public decimal sell_qtty { get; set; }
        public decimal sell_val { get; set; }
        public decimal buy_qtty { get; set; }
        public decimal buy_val { get; set; }
        public decimal net_val { get; set; }
    }
}
