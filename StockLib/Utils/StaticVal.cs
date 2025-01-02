using Binance.Net.Clients;
using Bybit.Net.Clients;
using StockLib.DAL.Entity;

namespace StockLib.Utils
{
    public static class StaticVal
    {
        private static BybitRestClient _bybit;
        private static BybitSocketClient _bybitSocket;
        private static BinanceRestClient _binance;
        private static BinanceSocketClient _binanceSocket;

        public static (long, long, long, long) _currentTime;//yearquarter + year + quarter + flag
        public static int _TAKE = 15;
        public static List<Stock> _lStock = new List<Stock>();
        public static int _MaxRate = 500;
        public static string _VietStock_Cookie = "ASP.NET_SessionId=nlehv103ci5raqevopu2cysl; __RequestVerificationToken=Mmg6CGomYCXe4BZkIkoJRLvUmhZ99CUb4coeUeaymZCN9xYHumQFNeWQBeu2TlJriagiaA-3vecniiRfEVRWQBIsYA-Gi-PysFMLQ8FjpXY1; language=vi-VN; _ga=GA1.1.1634064005.1724032252; _ga_EXMM0DKVEX=GS1.1.1724032251.1.0.1724032251.60.0.0; AnonymousNotification=; Theme=Light; vts_usr_lg=8E7FC9B57A7F485E1BF6E003F8700B04A197873D535172F8B72CBF3E943E96E892AE7919BFE55F31E327735F1C715D801A1EEA9F5DEE728F9642A841504B610E1D6B04D958309DAEDE842A620B055DDD0480800FD63C2D047B2CB31F794DC93F9B7E7116E87AFEDB3CF0C90934AC0B47950BC5585E235CFF35E59885BEFCDCCE";
        public static string _VietStock_Token = "wVIyNRGpnFhrFclsY80ON85OurU8C1z0U53Yhn8uPuHtKkP2RNX7XMWZQXaP3xTANcAMaFUCAcCkgD5lAxbLRJ6t89Ui-MFsrh90SL6z57ygdjrSXm9sxaLvFZCYx0im0";
        public static int _curQuarter = 20243;

        public static BybitRestClient ByBitInstance()
        {
            if (_bybit == null)
            {
                _bybit = new BybitRestClient();
            }
            return _bybit;
        }

        public static BybitSocketClient BybitSocketInstance()
        {
            if (_bybitSocket == null)
            {
                _bybitSocket = new BybitSocketClient();
            }
            return _bybitSocket;
        }


        public static BinanceRestClient BinanceInstance()
        {
            if (_binance == null)
            {
                _binance = new BinanceRestClient();
            }
            return _binance;
        }

        public static BinanceSocketClient BinanceSocketInstance()
        {
            if (_binanceSocket == null)
            {
                _binanceSocket = new BinanceSocketClient();
            }
            return _binanceSocket;
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
