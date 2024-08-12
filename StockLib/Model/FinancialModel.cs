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
}
