using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Stock : BaseDTO
    {
        public string s { get; set; }//symbol
        public string e { get; set; }//exchange
        public CompanyProfileDataModel p { get; set; } //profile
        public List<H24DataModel> h24 { get; set; }//nhóm ngành lấy từ 24h smart money
        public List<BCTCReleaseModel> bc { get; set; }//Thời gian release BCTC --> sẽ bỏ khi làm xong
        public int rank { get; set; }
        public List<IndicatorModel> indicator { get; set; }
        public int status { get; set; }
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

    public class H24DataModel
    {
        public string code { get; set; }
        public string name { get; set; }
        public int level { get; set; }
    }

    public class BCTCReleaseModel
    {
        public long t { get; set; }
        public string title { get; set; }
    }

    public class IndicatorModel
    {
        public int type { get; set; }//Loại chỉ báo
        public double avg_rate { get; set; }//Trung bình mỗi lần vào lệnh
        public double win_rate { get; set; }//Tỉ lệ winrate
        public double loss_rate { get; set; }//Tỉ lệ lossrate
    }
}
