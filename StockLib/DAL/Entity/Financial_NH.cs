using MongoDB.Bson.Serialization.Attributes;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_NH : BaseFinancialDTO
    {
        public double risk { get; set; }//Trích lập dự phòng
        public double nim_r { get; set; }//Lãi thuần / Tổng tài sản sinh lãi
        public double casa_r { get; set; }//Tiền gửi KH không kỳ hạn/ Tổng tiền gửi KH
        public double cir_r { get; set; }//Tổng chi phí hoạt động/ Tổng thu nhập hoạt động 
        public double credit_r { get; set; }//Tăng trưởng tín dụng
        public double cost_r { get; set; }//Giảm chi phí vốn(chi phí lãi và các chi phí tương tự)
        public int debt { get; set; }//Tổng nợ 
        public int debt1 { get; set; }//Nợ mức 1
        public int debt2 { get; set; }//Nợ mức 2
        public int debt3 { get; set; }//Nợ mức 3
        public int debt4 { get; set; }//Nợ mức 4
        public int debt5 { get; set; }//Nợ mức 5
        public double cover_r { get; set; }//Tỉ lệ bao phủ nợ xấu
    }
}
