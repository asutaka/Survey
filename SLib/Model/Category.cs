using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class Category : BaseMongoDTO
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
