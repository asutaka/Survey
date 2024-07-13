using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class NhomNganhAPIModel
    {
        public NhomNganhAPIData data { get; set; }
        public int status { get; set; }
    }

    public class NhomNganhAPIData
    {
        public List<NhomNganhAPIGroup> groups { get; set; }
        public long last_update { get; set; }
    }

    public class NhomNganhAPIGroup
    {
        public string icb_code { get; set; }
        public string icb_name { get; set; }
        public int icb_level { get; set; }
        public List<NhomNganhAPIGroup> child { get; set; }
        public int total_stock { get; set; }
        public int total_stock_increase { get; set; }
        public int total_stock_nochange { get; set; }
        public int toal_stock_decrease { get; set; }
        public decimal avg_change_percent { get; set; }
        public decimal total_val { get; set; }
        public decimal toal_val_increase { get; set; }
        public decimal total_val_nochange { get; set; }
        public decimal total_val_decrease { get; set; }
    }
}
