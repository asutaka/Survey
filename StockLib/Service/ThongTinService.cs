namespace StockLib.Service
{
    public interface IThongTinService
    {
        string Mes_ThongTinCoPhieu(string code);
    }
    public class ThongTinService : IThongTinService
    {
        public string Mes_ThongTinCoPhieu(string code)
        {
            if (code.Equals("PVD"))
            {
                return $"{code}: Cho thuê giàn khoan";
            }
            else if (code.Equals("PVS"))
            {
                return $"{code}: Thăm dò và khai thác dầu khí";
            }
            else if (code.Equals("PVT") || code.Equals("PVP"))
            {
                return $"{code}: Vận tải dầu khí";
            }
            else if (code.Equals("GAS"))
            {
                return $"{code}: Chế biến và phân phối khí(mua trong nước + nhập khẩu)";
            }
            else if (code.Equals("BSR"))
            {
                return $"{code}: Chế biến dầu mỏ(100% nhập khẩu)";
            }
            else if (code.Equals("POW"))
            {
                return $"{code}: Điện khí(khí là nguyên liệu đầu vào)";
            }
            else if (code.Equals("DPM") || code.Equals("DCM"))
            {
                return $"{code}: Dùng khí để tổng hợp NH3";
            }
            else if (code.Equals("PLX"))
            {
                return $"{code}: Phân phối dầu khí(chiếm 50% thị phần)";
            }
            else if (code.Equals("OIL"))
            {
                return $"{code}: Phân phối dầu khí(chiếm 20% thị phần)";
            }
            return string.Empty;
        }
    }
}