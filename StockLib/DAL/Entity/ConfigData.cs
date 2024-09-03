using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class ConfigData : BaseDTO
    {
        public int ty { get; set; }//type
        public long t { get; set; }//time
        public int va { get; set; }//Dùng khi cần thêm điều kiện check
    }
}
