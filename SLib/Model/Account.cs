using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class Account : BaseMongoDTO
    {
    }
}
