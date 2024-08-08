using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_NH : BaseFinancialDTO
    {
        public double cost_o { get; set; }//Chi phí hoạt động
        public double cost_r { get; set; }//Trích lập dự phòng
        public double nim { get; set; }
        public double casa { get; set; }
        public double cir { get; set; }
        public double lend { get; set; }//Cho vay
        public int debt { get; set; }//Tổng nợ
        public int debt1 { get; set; }//Nợ mức 1
        public int debt2 { get; set; }//Nợ mức 2
        public int debt3 { get; set; }//Nợ mức 3
        public int debt4 { get; set; }//Nợ mức 4
        public int debt5 { get; set; }//Nợ mức 5
    }
}
