using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class SpecialInfo : BaseDTO
    {
        public string s { get; set; }
        public List<string> locate { get; set; }//dùng cho các cp KCN
        public List<SpecialStock> stocks { get; set; }//dùng cho các cp chứng khoán
        public List<string> materials { get; set; }//nguyên liệu chính: dùng cho phân bón
        public List<string> products { get; set; }//sản phẩm chính: dùng cho hóa chất
    }

    public class SpecialStock
    {
        public string s { get; set; }
        public double rate { get; set; }
    }
}
