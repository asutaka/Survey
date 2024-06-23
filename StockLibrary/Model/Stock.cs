using MongoDB.Bson.Serialization.Attributes;
using StockLibrary.Model.APIModel;
using System.Collections.Generic;

namespace StockLibrary.Model
{
    [BsonIgnoreExtraElements]
    public class Stock : BaseMongoDTO
    {
        [BsonElement("ma_ck")]
        public string MaCK { get; set; }
        [BsonElement("san_ck")]
        public string SanCK { get; set; }
        public CompanyProfileDataModel profile { get; set; }
        public List<ShareHolderDataModel> share_holders { get; set; }
    }
}
