using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class ThongKeHaiQuan : BaseDTO
    {
        public int d { get; set; }
        public int key { get; set; }
        public double va { get; set; }
        public double price { get; set; }
    }
}
