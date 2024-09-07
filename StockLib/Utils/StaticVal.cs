using StockLib.DAL.Entity;

namespace StockLib.Utils
{
    public static class StaticVal
    {
        public static (long, long, long, long) _currentTime;//yearquarter + year + quarter + flag
        public static int _TAKE = 15;
        public static List<Stock> _lStock = new List<Stock>();
        public static int _MaxRate = 500;

        public static List<string> _lNganHang = new List<string>
        {
            "NH",
            "NganHang",
            "Ngan Hang",
            "Ngân hàng",
            "Bank"
        };

        public static List<string> _lBatDongSan = new List<string>
        {
            "BDS",
            "Batdongsan",
            "Bat dong san",
            "Bất động sản"
        };

        public static List<string> _lChungKhoan = new List<string>
        {
            "CK",
            "ChungKhoan",
            "Chung Khoan",
            "Chứng khoán"
        };

        public static List<string> _lThep = new List<string>
        {
            "Thep",
            "Thép",
            "Theps"
        };

        public static List<string> _lBanLe = new List<string>
        {
            "BanLe",
            "Bán Lẻ",
            "Ban Le"
        };

        public static List<string> _lDien = new List<string>
        {
            "Dien",
            "Điện"
        };

        public static List<string> _lKCN = new List<string>
        {
            "VGC",
            "KBC",
            "ITA",
            "TIX",
            "LHG",
            "BCM",
            "SZL",
            "D2D",
            "SZC",
            "CCL",
            "DRH",
            "HAR",
            "KOS",
            "VC3",
            "IDV",
            "IDC",
            "BII",
            "HPI",
            "NTC",
            "SIP",
            "SNZ",
            "TID"
        };

        public static List<string> _lVin = new List<string>
        {
            "VIC",
            "VHM",
            "VRE"
        };

        public static List<string> _lXayDung = new List<string>
        {
            "CTD",
            "FCN",
            "HBC"
        };

        public static List<string> _lThuySan = new List<string>
        {
            "AAM",
            "ACL",
            "AGF",
            "ANV",
            "APT",
            "ASM",
            "ATA",
            "AVF",
            "BLF",
            "CAD",
            "CAT",
            "CCA",
            "CMX",
            "DAT",
            "DMN",
            "FMC",
            "HVG",
            "ICF",
            "IDI",
            "JOS",
            "KHS",
            "MPC",
            "NGC",
            "SEA",
            "SNC",
            "SPD",
            "SPV",
            "SSN",
            "TCJ",
            "THP",
            "TS4",
            "VHC",
            "VNH"
        };

        public static List<string> _lXimang = new List<string>
        {
            "BCC",
            "BTS",
            "CCM",
            "CLH",
            "HOM",
            "HT1",
            "HVX",
            "LCC",
            "PTE",
            "PX1",
            "QNC",
            "SCJ",
            "SDY",
            "TBX",
            "TMX",
            "TXM",
            "VCX",
            "YBC"
        };

        public static List<string> _lNhua = new List<string>
        {
            "BMP",
            "NTP",
            "AAA",
            "DAG",
            "NHH",
            "NSG",
            "PCH",
            "PGN",
            "PLP",
            "TPC",
            "VKP",
            "VNP",
            "VTZ"
        };

        public static List<string> _lCaoSu = new List<string>
        {
            "BRC",
            "BRR",
            "DPR",
            "DRG",
            "DRI",
            "GVR",
            "HRC",
            "IRC",
            "PHR",
            "RBC",
            "RTB",
            "SBR",
            "TNC",
            "TRC"
        };

        public static List<string> _lPhanBon = new List<string>
        {
            "BFC",
            "CSV",
            "DCM",
            "DDV",
            "DGC",
            "DHB",
            "DPM",
            "HPH",
            "HSI",
            "HVT",
            "LAS",
            "NFC",
            "PAT",
            "PCE",
            "PMB",
            "PSE",
            "PSW",
            "SFG",
            "VAF"
        };
    }
}
