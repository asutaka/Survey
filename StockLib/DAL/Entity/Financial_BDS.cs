using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_BDS : BaseFinancialDTO
    {
        public double inv { get; set; }//Tồn Kho
        public double bp { get; set; }//Người mua trả tiền trước
        public double tl { get; set; }//Tổng nợ
        public double eq { get; set; }//Vốn chủ sở hữu
        public double ce { get; set; }//Giá vốn 
    }
}
