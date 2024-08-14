using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_CK : BaseFinancialDTO
    {
        public double broker { get; set; } //Doanh thu môi giới
        public double bcost { get; set; } //Chi phí môi giới
        public double debt { get; set; } //Các khoản cho vay(phần rất lớn là tăng trưởng margin)(CDKT)
        public double idebt { get; set; } //Lãi từ các khoản cho vay
        public double trade { get; set; } //Doanh thu tự doanh
        public double itrade { get; set; } //Tài sản tự doanh
        public double eq { get; set; } //Vốn chủ sở hữu(CDKT)
    }
}
