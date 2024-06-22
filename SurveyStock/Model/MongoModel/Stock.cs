using MongoDB.Bson.Serialization.Attributes;
using SurveyStock.Model.APIModel;
using System.Collections.Generic;

namespace SurveyStock.Model.MongoModel
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
