namespace StockLib.Model
{
    public class HighChart_LoiNhuanModel
    {
        public string s { get; set; }
        public int d { get; set; }
        
        public double DoanhThu { get; set; }
        public double LoiNhuan { get; set; }
        public double TangTruongDoanhThu { get; set; }
        public double TangTruongLoiNhuan { get; set; }
        public double TySuatLoiNhuan { get; set; }
    }

    public class HighChart_TinDung
    {
        public string s { get; set; }
        public int d { get; set; }

        public double TangTruongTinDung { get; set; }
        public double RoomTinDung { get; set; }
    }

    public class HighChart_NoXau
    {
        public string s { get; set; }
        public int d { get; set; }

        public double TongNoXau { get; set; }
        public double NoNhom1 { get; set; }
        public double NoNhom2 { get; set; }
        public double NoNhom3 { get; set; }
        public double NoNhom4 { get; set; }
        public double NoNhom5 { get; set; }
        public double TrichLapDuPhong { get; set; }
    }
}
