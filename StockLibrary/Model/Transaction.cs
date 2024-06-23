using MongoDB.Bson.Serialization.Attributes;
using System;

namespace StockLibrary.Model
{
    [BsonIgnoreExtraElements]
    public class Transaction : BaseMongoDTO
    {
        public int stt { get; set; }
        public DateTime ngay { get; set; }
        public string type { get; set; }
        public string ma_ck { get; set; }
        public int kl_mua { get; set; }
        public int kl_ban { get; set; }
        public int giatri_mua { get; set; }
        public int giatri_ban { get; set; }
        public DateTime create_at { get; set; }
        public bool recheck { get; set; }
    }
}
