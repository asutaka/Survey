using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class Stock : BaseMongoDTO
    {
        public string s { get; set; }//symbol
        public string e { get; set; }//exchange
        public CompanyProfileDataModel p { get; set; } //profile
        public List<string> cat { get; set; }//cat
        public List<string> catl { get; set; }//cat leader
    }

    public class CompanyProfileDataModel
    {
        public string i { get; set; }//industryName
        public string sups { get; set; }//superSector
        public string sec { get; set; }//sector
        public string subs { get; set; }//subSector
        public string fd { get; set; }//Năm thành lập(foundingDate)
        public decimal cc { get; set; }//Vốn điều lệ(charterCapital)
        public int noe { get; set; }//Số lượng nhân viên(numberOfEmployee)
        public string ld { get; set; }//Ngày niêm yết(listingDate)
        public decimal fp { get; set; }//Giá chào sàn(firstPrice)
        public decimal iss { get; set; }//KL đang niêm yết(issueShare)
        public decimal lv { get; set; }//Vốn hóa(listedValue)
        public string n { get; set; }//companyName
        public decimal q { get; set; }//SLCP lưu hành(quantity)
    }
}
