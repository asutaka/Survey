using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class StockType : BaseDTO
    {
        public string s { get; set; }//symbol
        public int ty1 { get; set; }
        public int ty1_r { get; set; }
        public int ty2 { get; set; }
        public int ty2_r { get; set; }
        public int ty3 { get; set; }
        public int ty3_r { get; set; }
    }
}
