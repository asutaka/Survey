using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SLib.Model
{
    [BsonIgnoreExtraElements]
    public class GoogleSheet : BaseMongoDTO
    {
        public string n { get; set; }//Name
        public string fn { get; set; }//FileName
        public int ty { get; set; }//type: 1: Nhóm ngành; 2: Cổ phiếu
        public string ssi { get; set; }//SpreadSheetID
        public List<string> sh { get; set; }
    }
}
