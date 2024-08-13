using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_BDS : BaseFinancialDTO
    {
        public int type { get; set; }//0: BDS nhà ở; 1: BDS KCN
        public double inv { get; set; }//Tồn Kho
        public double bp { get; set; }//Người mua trả tiền trước
        public double debt { get; set; }//Nợ tài chính
        public double eq { get; set; }//Vốn chủ sở hữu
        public double ce { get; set; }//Giá vốn 
    }
}
