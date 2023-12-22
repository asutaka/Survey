using System;
using System.Collections.Generic;

namespace Survey.Models
{
    public class AppsettingModel
    {
        public APIModel API { get; set; }
        public ViewWebModel ViewWeb { get; set; }
    }

    public class APIModel
    {
        public string API24hr { get; set; }
        public string Coin { get; set; }
        public string History1H { get; set; }
        public string History15M { get; set; }
        public string History1HTime { get; set; }
        public string History15MTime { get; set; }
    }

    public class ViewWebModel
    {
        public string Single { get; set; }
    }

    public class UserDataModel
    {
        public List<UserDataCoinModel> FOLLOW { get; set; }
    }

    public class UserDataCoinModel
    {
        public string Coin { get; set; }
        public decimal Buy { get; set; }
        public decimal Value { get; set; }
    }

    public class TraceCoinModel : UserDataCoinModel
    {
        public int STT { get; set; }
        public decimal CurValue { get; set; }
        public decimal DivValue { get; set; }
        public string RatioValue { get; set; }
    }



    public class TicketModel
    {
        public string symbol { get; set; }
        public float priceChange { get; set; }
        public float priceChangePercent { get; set; }
        public float weightedAvgPrice { get; set; }
        public float prevClosePrice { get; set; }
        public float lastPrice { get; set; }
        public float lastQty { get; set; }
        public float bidPrice { get; set; }
        public float bidQty { get; set; }
        public float askPrice { get; set; }
        public float askQty { get; set; }
        public float openPrice { get; set; }
        public float highPrice { get; set; }
        public float lowPrice { get; set; }
        public float volume { get; set; }
        public float quoteVolume { get; set; }
        public long openTime { get; set; }
        public long closeTime { get; set; }
        public long firstId { get; set; }
        public long lastId { get; set; }
        public long count { get; set; }
    }

    public class FinancialDataPoint
    {
        public DateTime DateTimeStamp { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public bool IsEmpty { get { return DateTimeStamp.Equals(new DateTime()); } }
        public double MCDX { get; set; }
        public string Description { get; set; }

        public FinancialDataPoint() { }
        public FinancialDataPoint(DateTime date, double open, double high, double low, double close, double volume)
        {
            this.DateTimeStamp = date;
            this.Low = low;
            this.High = high;
            this.Open = open;
            this.Close = close;
            this.Volume = volume;
        }
    }

    public class CryptonDataModel
    {
        public List<CryptonDetailDataModel> Data { get; set; }
    }

    public class CryptonDetailDataModel
    {
        public string S { get; set; }
        public string AN { get; set; }
    }
}
