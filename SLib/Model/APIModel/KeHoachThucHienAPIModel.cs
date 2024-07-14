using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class KeHoachThucHienAPIModel
    {
        public int status { get; set; }
        public List<KeHoachThucHienAPIData> data { get; set; }
    }

    public class KeHoachThucHienAPIData
    {
        public int year { get; set; }
        public decimal isa3 { get; set; }
        public decimal isa16 { get; set; }
        public decimal isa22 { get; set; }
        public List<KeHoachThucHienAPIQuarter> quarter { get; set; }
    }

    public class KeHoachThucHienAPIQuarter
    {
        public int quarter { get; set; }
        public decimal isa3_report { get; set; }
        public decimal isa3_percent { get; set; }
        public decimal isa16_report { get; set; }
        public decimal isa16_percent { get; set; }
        public decimal isa22_report { get; set; }
        public decimal isa22_percent { get; set; }
    }
}
