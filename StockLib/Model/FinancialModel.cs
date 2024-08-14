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
        public double BaoPhuNoXau { get; set; }
        public double TileNoXau { get; set; }
        public double TangTruongTrichLap { get; set; }
        public double TangTruongNoNhom2 { get; set; }
    }

    public class HighChart_NimCasa
    {
        public string s { get; set; }
        public int d { get; set; }

        public double Nim { get; set; }
        public double Casa { get; set; }
        public double Cir { get; set; }
        public double ChiPhiVon { get; set; }
    }

    public class HighChart_TonKho
    {
        public string s { get; set; }
        public int d { get; set; }
        public double TonKho { get; set; }
        public double TangTruong { get; set; }
    }

    public class HighChart_NguoiMua
    {
        public string s { get; set; }
        public int d { get; set; }
        public double NguoiMua { get; set; }
        public double TangTruong { get; set; }
    }

    public class HighChart_NoTrenVonChu
    {
        public string s { get; set; }
        public int d { get; set; }
        public double VonChu { get; set; }
        public double No { get; set; }
        public double NoTrenVonChu { get; set; }
    }
}
