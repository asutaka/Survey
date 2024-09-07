using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_DauTuCong : BaseFinancialDTO
    {
        public double debt { get; set; }//Nợ tài chính
        public double eq { get; set; }//Vốn chủ sở hữu
    }
}
