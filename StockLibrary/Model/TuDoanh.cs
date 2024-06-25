using MongoDB.Bson.Serialization.Attributes;
using System;

namespace StockLibrary.Model
{
    [BsonIgnoreExtraElements]
    public class TuDoanh : BaseMongoDTO
    {
        public int stt { get; set; }
        public long d { get; set; }
        public string type { get; set; }
        public string ma_ck { get; set; }
        public int kl_mua { get; set; }
        public int kl_ban { get; set; }
        public int giatri_mua { get; set; }
        public int giatri_ban { get; set; }
        public long t { get; set; }
        public bool recheck { get; set; }
    }
}
