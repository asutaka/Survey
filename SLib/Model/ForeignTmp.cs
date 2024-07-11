using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class ForeignTmp : BaseMongoDTO
    {
        public long d { get; set; }//tradingDate
        public decimal pc { get; set; }//priceChange
        public decimal ppc { get; set; }//perPriceChange
        public decimal cp { get; set; }//ceilingPrice
        public decimal fp { get; set; }//floorPrice
        public decimal rp { get; set; }//refPrice
        public decimal o { get; set; }//openPrice
        public decimal h { get; set; }//highestPrice
        public decimal l { get; set; }//lowestPrice
        public decimal c { get; set; }//closePrice
        public decimal ap { get; set; }//averagePrice
        public decimal cpa { get; set; }//closePriceAdjusted
        public decimal tmvo { get; set; }//totalMatchVol
        public decimal tmva { get; set; }//totalMatchVal
        public decimal tdva { get; set; }//totalDealVal
        public decimal tdvo { get; set; }//totalDealVol
        public decimal fbvot { get; set; }//foreignBuyVolTotal
        public decimal fcr { get; set; }//foreignCurrentRoom
        public decimal fsvot { get; set; }//foreignSellVolTotal
        public decimal fbvat { get; set; }//foreignBuyValTotal
        public decimal fsvat { get; set; }//foreignSellValTotal
        public decimal tbt { get; set; }//totalBuyTrade
        public decimal tbtvo { get; set; }//totalBuyTradeVol
        public decimal tst { get; set; }//totalSellTrade
        public decimal tstvo { get; set; }//totalSellTradeVol
        public decimal nbsvo { get; set; }//netBuySellVol
        public decimal nbsva { get; set; }//netBuySellVal
        public decimal ex { get; set; }//exchange
        public string s { get; set; }//symbol
        public decimal fbvom { get; set; }//foreignBuyVolMatched
        public decimal fbvod { get; set; }//foreignBuyVolDeal
        public long t { get; set; }//create_at
    }
}
