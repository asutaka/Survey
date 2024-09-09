using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Share : BaseDTO
    {
        public string s { get; set; }
        public double share { get; set; }
    }
}
