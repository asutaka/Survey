using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class ConfigBCTC : BaseMongoDTO
    {
        public int type { get; set; }
        public List<ConfigBCTCData> data { get; set; }
    }

    public class ConfigBCTCData
    {
        public int code { get; set; }
        public string name { get; set; }
        public bool status { get; set; }
    }
}
