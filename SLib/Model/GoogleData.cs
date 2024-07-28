using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class GoogleData : BaseMongoDTO
    {
        public long t { get; set; }
        public int ty { get; set; }//0: bản ghi giá trị, 1: bản ghi tỉ lệ
        public string nhom { get; set; }
        public string sheet { get; set; }
        public string code { get; set; }
        public int year { get; set; }
        public int quarter { get; set; }
        public decimal value { get; set; }
        public decimal rate { get; set; }
    }
}
