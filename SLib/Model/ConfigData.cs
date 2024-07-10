using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class ConfigData : BaseMongoDTO
    {
        public int ty { get; set; }//type
        public long t { get; set; }//time
    }
}
