using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.DAL.Entity
{
    [BsonIgnoreExtraElements]
    public class Financial_Logistic : BaseFinancialDTO
    {
        public double debt { get; set; }//Nợ tài chính
        public double eq { get; set; }//Vốn chủ sở hữu
    }
}
