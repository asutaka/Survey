using MongoDB.Bson.Serialization.Attributes;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class Financial : BaseMongoDTO
    {
        public long d { get; set; }
        public string s { get; set; }
        public long t { get; set; }
        public decimal? revenue { get; set; }
        public decimal? profit { get; set; }
        public int yearReport { get; set; }
        public int lengthReport { get; set; }
        public decimal eps { get; set; }
        public decimal diluteEPS { get; set; }
        public decimal pe { get; set; }
        public decimal dilutedPe { get; set; }
        public decimal roe { get; set; }
        public decimal roa { get; set; }
        public decimal roic { get; set; }
        public decimal grossProfitMargin { get; set; }
        public decimal netProfitMargin { get; set; }
        public decimal debtEquity { get; set; }
        public decimal debtAsset { get; set; }
        public decimal quickRatio { get; set; }
        public decimal currentRatio { get; set; }
        public decimal pb { get; set; }
    }
}
