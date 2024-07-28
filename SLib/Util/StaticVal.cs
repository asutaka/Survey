using SLib.Model.GoogleSheetModel;
using System.Collections.Generic;

namespace SLib.Util
{
    public static class StaticVal
    {
        public static List<string> lKeyword = new List<string>
        {
            "ĐÁNH GIÁ",
            "MUA",
            "BÁN",
            "NGÀNH",
            "HÔM NAY"
        };

        public static Dictionary<string, List<string>> lHotKey = new Dictionary<string, List<string>>
        {
            {"8500", new List<string>{ "Bảo hiểm", "bh", "bao hiem" } },
            {"8600", new List<string>{ "Bất động sản","bds", "batdongsan", "bat dong san" } },
            {"8777", new List<string>{ "Chứng khoán","stock", "ck", "chungkhoan", "chung khoan" } },
            {"8300", new List<string>{ "Ngân hàng", "nh", "bank", "nganhang", "ngan hang" } },
            {"5379", new List<string>{ "Bán lẻ", "bl", "ban le", "banle" } },
            {"1353", new List<string>{ "Nhựa cao su", "cao su", "cs", "nhua cao su", "nhuacaosu", "caosu" } },
            {"1357", new List<string>{ "Hoá chất", "phân bón", "hc", "pb", "phanbon", "hoachat", "phan bon", "hoa chat" } },
            {"1733", new List<string>{ "Gỗ", "go" } },
            {"1757", new List<string>{ "Thép", "th", "thep", "steel", "st" } },
            {"0500", new List<string>{ "Dầu khí", "dk", "daukhi", "dau khi" } },
            {"2353", new List<string>{ "Xi măng", "xm", "xi mang", "ximang" } },
            {"2357", new List<string>{ "Xây dựng", "xd", "xay dung", "xaydung" } },
            {"2777", new List<string>{ "Cảng biển", "cb", "cangbien", "cang bien" } },
            {"4500", new List<string>{ "Dược", "du", "duoc" } },
            {"7535", new List<string>{ "Năng lượng", "nl", "nang luong", "nangluong" } }
        };

        public static Dictionary<string, string> lHotKeyGG = new Dictionary<string, string>
        {
            {"8600", "BatDongSan" },
            {"8777", "ChungKhoan" },
            {"8300", "NganHang" },
            {"5379", "BanLe" },
            {"1353", "CaoSu" },
            {"1357", "PhanBon_HoaChat" },
            {"1757", "Thep" },
            {"0500", "DauKhi" },
            {"2777", "CangBien_VanTaiBien" },
            {"7535", "NangLuong" }
        };

        public static List<string> lTangTruong = new List<string>
        {
            "Công nghệ - viễn thông",
            "Chứng khoán",
            "Dầu khí",
            "Cảng biển - vận tải dầu khí"
        };

        public static List<string> lPhucHoi = new List<string>
        {
            "Ngân hàng",
            "Thép",
            "Năng lượng",
            "Xuất khẩu",
            "Đầu tư công",
            "Khu công nghiệp"
        };

        public static List<string> lTaoDay = new List<string>
        {
            "Bán lẻ",
            "Bất động sản",
            "Hàng không"
        };
    }
}
