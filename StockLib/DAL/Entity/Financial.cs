using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial : BaseDTO
    {
        public string s { get; set; }//Mã CK
        public int d { get; set; }//Năm - Quý
        public int t { get; set; }//Thời gian cập nhật
        public int pl { get; set; }//Ngày public BCTC
        public double rv { get; set; }//Doanh thu
        public double pf { get; set; }//Lợi nhuận
        public double pfg { get; set; }//Lợi nhuận Gộp 
        public double pfn { get; set; }//Lợi nhuận Ròng
        public double inv { get; set; }//Tồn Kho
        public double debt { get; set; }//Nợ tài chính
        public double eq { get; set; }//Vốn chủ sở hữu
        public double bp { get; set; }//Người mua trả tiền trước
    }
}
