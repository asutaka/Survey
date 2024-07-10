using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class Category : BaseMongoDTO
    {
    }
}
