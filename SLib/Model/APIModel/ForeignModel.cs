using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class ForeignModel
    {
        public List<ForeignDataModel> data { get; set; }
        public ForeignPageModel paging { get; set; }
    }

    public class ForeignPageModel
    {
        public int total { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }

    public class ForeignDataModel
    {
        public string tradingDate { get; set; }
        public decimal priceChange { get; set; }
        public decimal perPriceChange { get; set; }
        public decimal ceilingPrice { get; set; }
        public decimal floorPrice { get; set; }
        public decimal refPrice { get; set; }
        public decimal openPrice { get; set; }
        public decimal highestPrice { get; set; }
        public decimal lowestPrice { get; set; }
        public decimal closePrice { get; set; }
        public decimal averagePrice { get; set; }
        public decimal closePriceAdjusted { get; set; }
        public decimal totalMatchVol { get; set; }
        public decimal totalMatchVal { get; set; }
        public decimal totalDealVal { get; set; }
        public decimal totalDealVol { get; set; }
        public decimal foreignBuyVolTotal { get; set; }
        public decimal foreignCurrentRoom { get; set; }
        public decimal foreignSellVolTotal { get; set; }
        public decimal foreignBuyValTotal { get; set; }
        public decimal foreignSellValTotal { get; set; }
        public decimal totalBuyTrade { get; set; }
        public decimal totalBuyTradeVol { get; set; }
        public decimal totalSellTrade { get; set; }
        public decimal totalSellTradeVol { get; set; }
        public decimal netBuySellVol { get; set; }
        public decimal netBuySellVal { get; set; }
        public decimal exchange { get; set; }
        public string symbol { get; set; }
        public decimal foreignBuyVolMatched { get; set; }
        public decimal foreignBuyVolDeal { get; set; }
    }
    public class Foreign
    {
        public int no { get; set; }
        public long d { get; set; }
        public string s { get; set; }
        public decimal sell_qtty { get; set; }
        public decimal sell_val { get; set; }
        public decimal buy_qtty { get; set; }
        public decimal buy_val { get; set; }
        public decimal net_val { get; set; }
        public long t { get; set; }
    }
}

