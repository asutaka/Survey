using System;

namespace SLib.Model.GoogleSheetModel
{
    public class GoogleValueModel
    {
        public GoogleValueModel() {
            Time = DateTime.Now;
        }
        public DateTime Time { get; set; }
        public string NhomNganh { get; set; }
        public string SheetName { get; set; }
        public string Code { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public decimal Value { get; set; }
        public decimal Rate { get; set; }
    }
}
