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
        public double debt { get; set; }//Nợ tài chính - Đối với mã chứng khoán: Các khoản cho vay(phần rất lớn là tăng trưởng margin)(CDKT)
        public double eq { get; set; }//Vốn chủ sở hữu
        public double bp { get; set; }//Người mua trả tiền trước
        //Các property cho chứng khoán
        public double broker { get; set; } //Doanh thu môi giới
        public double bcost { get; set; } //Chi phí môi giới
        public double idebt { get; set; } //Lãi từ các khoản cho vay
        public double trade { get; set; } //Doanh thu tự doanh
        public double itrade { get; set; } //Tài sản tự doanh
    }
}
