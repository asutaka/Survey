using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class ThongKeQuy : BaseDTO
    {
        public int d { get; set; }
        public int key { get; set; }
        public string content { get; set; }
        public double va { get; set; }
        public string unit { get; set; }
        public double price { get; set; }
        public double qoq { get; set; }
        public double qoqoy { get; set; }
    }
}
