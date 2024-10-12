using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Coin : BaseDTO
    {
        public string s { get; set; }
        public List<IndicatorCoin> indicator { get; set; }
        public int status { get; set; }
    }

    public class IndicatorCoin
    {
        public int ty { get; set; }//Loại chỉ báo
        public int rank { get; set; }
        public double avg { get; set; }//TP Trung bình mỗi lần vào lệnh
        public double total { get; set; }//Tổng TP
        public int num { get; set; }//Số nến nắm giữ TB
        public int win { get; set; }//Xác suất win
        public int loss { get; set; }//Xác suất loss
    }
}
