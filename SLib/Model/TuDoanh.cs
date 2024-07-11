using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class TuDoanh : BaseMongoDTO
    {
        public int no { get; set; }
        public long d { get; set; }
        public string s { get; set; }
        public decimal net_deal { get; set; }//gia tri mua bán khớp lệnh
        public decimal net_pt { get; set; }//gia tri mua bán thỏa thuận
        public decimal net { get; set; }//gia tri mua bán
        public long t { get; set; }
    }
}
