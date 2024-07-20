using MongoDB.Bson.Serialization.Attributes;
using System;
namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class UserMessage : BaseMongoDTO
    {
        public long u { get; set; }
        public DateTime t { get; set; }
    }
}
