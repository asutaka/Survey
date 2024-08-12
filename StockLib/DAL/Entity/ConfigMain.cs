using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class ConfigMain : BaseDTO
    {
        public int quarter { get; set; }
        public int year { get; set; }
    }
}
