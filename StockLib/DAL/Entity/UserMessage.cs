using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class UserMessage : BaseDTO
    {
        public long u { get; set; }
        public DateTime t { get; set; }
    }
}
