using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.Model.MongoModel
{
    [BsonIgnoreExtraElements]
    public class Transaction : BaseMongoDTO
    {
        [BsonElement("name")]
        public string Name { get; set; }
    }
}
