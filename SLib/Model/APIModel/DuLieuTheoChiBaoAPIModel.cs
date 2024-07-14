using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class DuLieuTheoChiBaoAPIModel
    {
        public int status { get; set; }
        public DuLieuTheoChiBaoAPIDataModel data { get; set; }
    }

    public class DuLieuTheoChiBaoAPIDataModel
    {
        public List<DuLieuTheoChiBaoAPIDataDetailModel> data { get; set; }
        public int total_symbol { get; set; }
    }
    public class DuLieuTheoChiBaoAPIDataDetailModel
    {
        public string symbol { get; set; }
        public string company_name { get; set; }
        public string floor { get; set; }
        public decimal pb { get; set; }
        public decimal pb4Q { get; set; }
        public decimal pe { get; set; }
        public decimal pe4Q { get; set; }
        public decimal eps { get; set; }
        public decimal eps4Q { get; set; }
        public decimal market_cap { get; set; }
        public decimal the_beta { get; set; }
        public decimal avg_trading_vol_10 { get; set; }
        public decimal avg_trading_vol_5 { get; set; }
        public decimal match_price_10 { get; set; }
        public decimal match_Price_5 { get; set; }
        public decimal roa { get; set; }
        public decimal roe { get; set; }
        public decimal match_price { get; set; }
        public decimal basic_price { get; set; }
        public decimal ceiling_price { get; set; }
        public decimal floor_price { get; set; }
        public decimal change { get; set; }
        public decimal change_percent { get; set; }
        public decimal accumylated_vol { get; set; }
        public string object_type { get; set; }
        public decimal change_price_percent_5 { get; set; }
        public decimal change_vol_percent_5 { get; set; }
    }
}
