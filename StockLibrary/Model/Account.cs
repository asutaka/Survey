using MongoDB.Bson.Serialization.Attributes;

namespace StockLibrary.Model
{
    [BsonIgnoreExtraElements]
    public class Account : BaseMongoDTO
    {
    }
}
