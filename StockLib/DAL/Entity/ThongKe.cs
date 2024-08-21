using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class ThongKe : BaseDTO
    {
        public int d { get; set; }
        public int key { get; set; }
        public string content { get; set; }
        public double va { get; set; }
        public double va2 { get; set; }
    }
}
