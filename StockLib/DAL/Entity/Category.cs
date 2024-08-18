using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Category : BaseDTO
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
