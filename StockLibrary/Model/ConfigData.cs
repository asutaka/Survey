using MongoDB.Bson.Serialization.Attributes;

namespace StockLibrary.Model
{
    [BsonIgnoreExtraElements]
    public class ConfigData : BaseMongoDTO
    {
        public int type { get; set; }//
        public long time { get; set; }
    }
}
