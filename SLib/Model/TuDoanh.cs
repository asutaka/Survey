using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class TuDoanh : BaseMongoDTO
    {
        public int no { get; set; }
        public long d { get; set; }
        public string s { get; set; }
        public int bvo { get; set; }//kl mua
        public int svo { get; set; }//kl ban
        public decimal bva { get; set; }//gia tri mua
        public decimal sva { get; set; }//gia tri ban
        public long t { get; set; }
    }
}
