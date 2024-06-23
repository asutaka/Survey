namespace StockLibrary.Model.APIModel
{
    public class CompanyProfileModel
    {
        public CompanyProfileDataModel data { get; set; }
    }

    public class CompanyProfileDataModel
    {
        public string symbol { get; set; }
        public string industryName { get; set; }
        public string superSector { get; set; }
        public string sector { get; set; }
        public string subSector { get; set; }
        public string foundingDate { get; set; }//Năm thành lập
        public decimal charterCapital { get; set; }//Vốn điều lệ
        public int numberOfEmployee { get; set; }//Số lượng nhân viên
        public string listingDate { get; set; }//Ngày niêm yết
        public string exchange { get; set; }//sàn
        public decimal firstPrice { get; set; }//Giá chào sàn
        public decimal issueShare { get; set; }//KL đang niêm yết
        public decimal listedValue { get; set; }//Vốn hóa
        public string companyName { get; set; }
        public decimal quantity { get; set; }//SLCP lưu hành
    }
}
