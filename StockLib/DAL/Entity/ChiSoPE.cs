using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class ChiSoPE : BaseDTO
    {
        public string s { get; set; }
        public int d { get; set; }
        public double eps { get; set; }
        public double pe { get; set; }
    }
}
