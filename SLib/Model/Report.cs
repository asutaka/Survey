using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class Report : BaseMongoDTO
    {
    }
}
