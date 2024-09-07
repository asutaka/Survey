using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_ThuySan : BaseFinancialDTO
    {
        public int type { get; set; }//0: Nông nghiệp; 1: Thủy sản
        public double inv { get; set; }//Tồn Kho
        public double debt { get; set; }//Nợ tài chính
        public double eq { get; set; }//Vốn chủ sở hữu
    }
}
