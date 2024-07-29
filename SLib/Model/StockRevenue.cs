using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class StockRevenue : BaseMongoDTO
    {
        public int d { get; set; }
        public string s { get; set; }
        public List<StockRevenueDetal> de { get; set; }
    }

    public class StockRevenueDetal
    {
        public string name { get; set; }
        public float rate { get; set; }
        public string ex { get; set; }
    }
}
