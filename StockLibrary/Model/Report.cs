using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;
using System;

namespace StockLibrary.Model
{
    [BsonIgnoreExtraElements]
    public class Report : BaseMongoDTO
    {
        public int mode { get; set; }//Ngày, Tuần
        public string s { get; set; }//Mã CK
        public DateTime d { get; set; }//Ngày
        public EmaResult ema10 { get; set; }
        public IchimokuResult ichi { get; set; }
        public MacdResult macd { get; set; }
        public BollingerBandsResult bb { get; set; }
        public long t { get; set; }

    }
}
