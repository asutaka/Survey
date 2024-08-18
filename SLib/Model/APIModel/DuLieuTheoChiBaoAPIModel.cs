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
        public decimal match_price { get; set; }//Giá hiện tại
        public decimal basic_price { get; set; }//Giá tham chiếu
        public decimal accumylated_vol { get; set; }//Vol
        public decimal change_vol_percent_5 { get; set; }//Thay đổi so với 5 phiên trước
    }
}
