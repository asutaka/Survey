using StockLib.DAL.Entity;

namespace StockLib.Utils
{
    public static class StaticVal
    {
        public static (long, long, long, long) _currentTime;//yearquarter + year + quarter + flag
        public static int _TAKE = 15;
        public static List<Stock> _lStock = new List<Stock>();
        public static int _MaxRate = 500;

        public static List<string> _lNganHangKey = new List<string>
        {
            "NH",
            "NganHang",
            "Ngan Hang",
            "Ngân hàng",
            "Bank"
        };

        public static List<string> _lBatDongSanKey = new List<string>
        {
            "BDS",
            "Batdongsan",
            "Bat dong san",
            "Bất động sản"
        };

        public static List<string> _lChungKhoanKey = new List<string>
        {
            "CK",
            "ChungKhoan",
            "Chung Khoan",
            "Chứng khoán"
        };

        public static List<string> _lThepKey = new List<string>
        {
            "Thep",
            "Thép",
            "Theps"
        };

        public static List<string> _lBanLeKey = new List<string>
        {
            "BanLe",
            "Bán Lẻ",
            "Ban Le"
        };

        public static List<string> _lDienKey = new List<string>
        {
            "Dien",
            "Điện"
        };

        public static List<string> _lHangKhongKey = new List<string>
        {
            "hk",
            "hang khong"
        };

        public static List<string> _lCangBienKey = new List<string>
        {
            "cb",
            "port",
            "Cang bien"
        };

        public static List<string> _lLogisticKey = new List<string>
        {
            "Logistic",
            "vt",
            "van tai"
        };

        public static List<string> _lKCN = new List<string>
        {
            "BCM",
            "KBC",
            "VGC",
            "IDC",
            "SIP",
            "SNZ",
            "KOS",
            "SZC",
            "TID",
            "NTC",
            "VC3",
            "ITA",
            "LHG",
            "IDV",
            "D2D",
            "SZL",
            "TIX",
            "HPI",
            "CCL",
            "HAR",
            "DRH",
            "BII",
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
            "VHC",
            "MPC",
            "SEA",
            "ANV",
            "ASM",
            "FMC",
            "IDI",
            "CMX",
            "ACL",
            "DAT",
            "HVG",
            "CCA",
            "SPV",
            "THP",
            "CAT",
            "KHS",
            "SNC",
            "SPD",
            "AGF",
            "AAM",
            "DMN",
            "SSN",
            "BLF",
            "ICF",
            "TCJ",
            "APT",
            "TS4",
            "AVF",
            "VNH",
            "CAD",
            "JOS",
            "ATA",
            "NGC"
        };

        public static List<string> _lXimang = new List<string>
        {
            "HT1",
            "BCC",
            "BTS",
            "QNC",
            "CCM",
            "HOM",
            "CLH",
            "SCJ",
            "PX1",
            "VCX",
            "HVX",
            "YBC",
            "TMX",
            "TBX",
            "PTE",
            "TXM",
            "SDY",
            "LCC"
        };

        public static List<string> _lNhua = new List<string>
        {
            "NTP",
            "BMP",
            "AAA",
            "APH",
            "NHH",
            "VTZ",
            "PLP",
            "PCH",
            "VNP",
            "TPC",
            "NSG",
            "DAG",
            "PGN",
            "VKP"
        };

        public static List<string> _lCaoSuKey = new List<string>
        {
            "cs",
            "cao su"
        };

        public static List<string> _lCaoSu = new List<string>
        {
            "GVR",
            "PHR",
            "DRC",
            "DPR",
            "RTB",
            "BRR",
            "DRG",
            "HRC",
            "TRC",
            "DRI",
            "TNC",
            "SBR",
            "BRC",
            "IRC",
            "RBC"
        };

        public static List<string> _lPhanBon = new List<string>
        {
            "DGC",
            "DCM",
            "DPM",
            "CSV",
            "DDV",
            "LAS",
            "BFC",
            "DHB",
            "PAT",
            "HVT",
            "SFG",
            "VAF",
            "NFC",
            "PCE",
            "HPH",
            "PSW",
            "PSE",
            "PMB",
            "HSI"
        };
    }
}
