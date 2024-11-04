using Binance.Net.Clients;
using Bybit.Net.Clients;
using StockLib.DAL.Entity;

namespace StockLib.Utils
{
    public static class StaticVal
    {
        private static BybitRestClient _bybit;
        private static BinanceRestClient _binance;

        public static (long, long, long, long) _currentTime;//yearquarter + year + quarter + flag
        public static int _TAKE = 15;
        public static List<Stock> _lStock = new List<Stock>();
        public static int _MaxRate = 500;
        public static string _VietStock_Cookie = "ASP.NET_SessionId=nlehv103ci5raqevopu2cysl; __RequestVerificationToken=Mmg6CGomYCXe4BZkIkoJRLvUmhZ99CUb4coeUeaymZCN9xYHumQFNeWQBeu2TlJriagiaA-3vecniiRfEVRWQBIsYA-Gi-PysFMLQ8FjpXY1; language=vi-VN; _ga=GA1.1.1634064005.1724032252; _ga_EXMM0DKVEX=GS1.1.1724032251.1.0.1724032251.60.0.0; AnonymousNotification=; vts_usr_lg=52C040862BC0349EDA1B2DA66AE813C88206458D4BAC4B32043686732900A9E24C8C5936093B71328910E617C3C30EB71BA43BF92676E4BA972935FD99885E43AA46FA6EB3FE4BFE593FEE6C984D1C6DD55B24B8D616E2CF59BBE4F08C9DEC5CDFB3215EE9052062E6C41F4A59DE2DED1D2D798B28E456A8B5B22E49535011E7; vst_usr_lg_token=fpMPFJNOCEiVQlOGb57RRg==;";
        public static string _VietStock_Token = "qCQvr6zkixbVAFUtk7tpKYf01EX0xkPvPNoNWCYhv3IsBWFCjAzObmRNJ7M0S7IR5gVnr5hhNUKn3s_TRA3wEwX7wNuvQA7bd37UHt5QFMyNZQtKlCSTNiNRHBigpYBZ0";
        public static int _curQuarter = 20243;

        public static BybitRestClient ByBitInstance()
        {
            if (_bybit == null)
            {
                _bybit = new BybitRestClient();
            }
            return _bybit;
        }


        public static BinanceRestClient BinanceInstance()
        {
            if (_binance == null)
            {
                _binance = new BinanceRestClient();
            }
            return _binance;
        }


        public static List<string> _lBanLeKey = new List<string>
        {
            "BanLe",
            "Bán Lẻ",
            "Ban Le"
        };

        public static List<string> _lBatDongSanKey = new List<string>
        {
            "BDS",
            "Batdongsan",
            "Bat dong san",
            "Bất động sản"
        };

        public static List<string> _lCangBienKey = new List<string>
        {
            "cb",
            "port",
            "Cang bien"
        };

        public static List<string> _lCaoSuKey = new List<string>
        {
            "cs",
            "cao su"
        };

        public static List<string> _lChungKhoanKey = new List<string>
        {
            "CK",
            "ChungKhoan",
            "Chung Khoan",
            "Chứng khoán"
        };

        public static List<string> _lDetMayKey = new List<string>
        {
            "dm",
            "det may"
        };

        public static List<string> _lDienKey = new List<string>
        {
            "Dien",
            "Điện"
        };

        public static List<string> _lGoKey = new List<string>
        {
            "go"
        };

        public static List<string> _lHangKhongKey = new List<string>
        {
            "hk",
            "hang khong"
        };

        public static List<string> _lLogisticKey = new List<string>
        {
            "Logistic",
            "vt",
            "van tai"
        };

        public static List<string> _lNganHangKey = new List<string>
        {
            "NH",
            "NganHang",
            "Ngan Hang",
            "Ngân hàng",
            "Bank"
        };

        public static List<string> _lNhuaKey = new List<string>
        {
            "nhua",
            "plastic"
        };

        public static List<string> _lOtoKey = new List<string>
        {
            "o to"
        };

        public static List<string> _lPhanBonKey = new List<string>
        {
            "pb",
            "phan bon"
        };

        public static List<string> _lThanKey = new List<string>
        {
            "than"
        };

        public static List<string> _lThepKey = new List<string>
        {
            "Thep",
            "Thép",
            "Theps"
        };

        public static List<string> _lThuySanKey = new List<string>
        {
            "ts",
            "thuy san"
        };

        public static List<string> _lXimangKey = new List<string>
        {
            "xm",
            "xi mang"
        };

        public static List<string> _lXayDungKey = new List<string>
        {
            "xd",
            "xay dung"
        };

        public static List<string> _lKCNKey = new List<string>
        {
            "kcn"
        };

        public static List<string> _lDauKhiKey = new List<string>
        {
            "dk",
            "dau khi"
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

        public static List<string> _lDNVayVonNuocNgoai = new List<string>
        {
            "VIC",
            "NVL",
            "PGV",
            "QTP",
            "BWE",
            "HSG",
            "MSN",
            "PVT",
            "PC1"
        };

        public static List<string> _lInvesting = new List<string>
        {
            "3bc34e2c78fe3ed469698b88b71fff01_1day.json"   //BAID - BDTI
        };
    }
}
