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
        public static string _VietStock_Cookie = "language=vi-VN; Theme=Light; AnonymousNotification=; _pbjs_userid_consent_data=3524755945110770; isShowLogin=true; _cc_id=ebc6cdbd24cca9cce954e5a06cd7c5ba; panoramaId_expiry=1725723245184; panoramaId=fd5342b4d12fb3581e7ae9fa6c9c185ca02ce77fd625f98a7fe8fe514a59dedf; panoramaIdType=panoDevice; dable_uid=44708328.1725118445229; ASP.NET_SessionId=4idyib4u1rc0s2ncgenkk0v1; __RequestVerificationToken=-ny803vk8mbbdJEb8UqRDphtPszvmVh7sd8Z-zlAZ6OrPuJrdLl_QfKTxEp3GdMqL062vaH4yhn_JekmcJCq_M6emBlw6XdQTujORLjarXY1; _gid=GA1.2.2103867253.1725700047; __gads=ID=233a7d148667e1d0:T=1725118446:RT=1725700048:S=ALNI_Ma47tgzbY4avM5Kh3fy7OE-Nh4I2A; __gpi=UID=00000ee624024111:T=1725118446:RT=1725700048:S=ALNI_MYnrHXcXrZynGqtzL7IIHChNazPjg; __eoi=ID=c2eb48e8812aa43b:T=1725118446:RT=1725700048:S=AA-AfjYRP5qzLwziftRemNEtd4VP; vts_usr_lg=CF10EE1042B1BCB6027AC613017129F2DCDE8545E712713080E95B0AC741666FA27489BC791B84BA08DC9CC18FF0F847802D811FE0949DA4D28AEF98E8B4FBC94E8F80BB34FE53794A962CC481E48B0112D3E6416196CF5256C7C62400E7E31D5089E4BB7CC9CFC92FD8296C30716719; vst_usr_lg_token=G/kfULB87EO5p4684UUcqQ==; finance_viewedstock=NLG,HSG,; _ga=GA1.2.1298724755.1725118440; cto_bundle=c5jFT19FTzhUT3ZDSDh6QncxWlZ4REtCUnRXTnMzaWFjWENCamcwUiUyRnNweTRRQWJOQmFLSHNGS3RFN3pHVVY5c0ExWk9Ud2IzVVc0QU1INUJFYXJvNmNSUEtBUU90SGdzSVlvc3c1Zk54TnFNbk5HWDVwQlM5dHlPNTM1Y0RNZFZVenp4QVhCUlVNVGtsJTJGQSUyQmpZSHFkQmhPNEpiNFY5JTJCU0pYMFJwcEZLcWV6WEE3WSUzRA; cto_bidid=l5gv1F92JTJCeiUyRm1UbUtvcTROZkZKc0ZYV09RJTJGTDgwJTJGNDZzNTYlMkZ6c1I4NUY2OGhkTkw3bHFNeHExd3Y3eVhrN1U1Z3ZwaSUyQnBQR1k3MWo0cnRiZ20lMkJkUHBnZ1R1OWlWU2JhNVM3ZlpKM0NTNGlUU2FjJTNE; _ga_EXMM0DKVEX=GS1.1.1725700047.6.1.1725700114.60.0.0";
        public static string _VietStock_Token = "Qbhz8FkEYfdGbjJ3nhDb2ra4Rnbg9Ws3VuLoCvG4KMAlpDgMLzDnw-3psxQ1_A6Frt6tHVeCA5cxgc53FnMKsQt5ysN-WnvyynxAE7wQuLpA0qnuLpZT74NIoQ7rqji30";

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
