using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class StockFinancial : BaseDTO
    {
        public int d { get; set; }//Năm - Quý
        public string s { get; set; }//Mã CK
        public int pl { get; set; }//Ngày public BCTC
        public int t { get; set; }
        public StockFinancialObject rv { get; set; }//Doanh thu
        public double pf { get; set; }//Lợi nhuận
        public StockFinancialObjectTime fi { get; set; }//Đầu tư tài chính
        public StockFinancialObject inv { get; set; }//Tồn Kho
        public double ta { get; set; }//Tổng tài sản
        public StockFinancialObject bp { get; set; }//Người mua trả tiền trước
        public StockFinancialObjectTime la { get; set; }//Nợ ngắn hạn
        public StockFinancialObjectTime lal { get; set; }//Nợ dài hạn
        public double tl { get; set; }//Tổng nợ
        public double eq { get; set; }//Vốn chủ sở hữu
        public StockFinancialObject ce { get; set; }//Giá vốn 
    }

    public class StockFinancialDetail
    {
        public string name { get; set; }
        public double va { get; set; }
        public double rate { get; set; }
    }

    public class StockFinancialDetailTime : StockFinancialDetail
    {
        public int d { get; set; }//Ngày đáo hạn yyMMdd
    }

    public class StockFinancialObject
    {
        public double va { get; set; }
        public List<StockFinancialDetail> ld { get; set; }
    }

    public class StockFinancialObjectTime
    {
        public double va { get; set; }
        public List<StockFinancialDetailTime> ld { get; set; }
    }
}
