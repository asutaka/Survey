namespace StockLibrary.Service
{
    public static class ServiceSetting
    {
        public const string _stockExchange = "https://bgapidatafeed.vps.com.vn/getlistckindex/{0}";
        public const string _companyInfo = "https://iboard-api.ssi.com.vn/statistics/company/company-profile?symbol={0}&language=vn"; //Thông tin cty, nhóm ngành
        public const string _shareHolder = "https://iboard-api.ssi.com.vn/statistics/company/shareholders?symbol={0}&language=vn&page=1&pageSize=100"; //Thông tin cổ đông
        public const string _foreignBuySell = "https://iboard-api.ssi.com.vn/statistics/company/stock-price?symbol={0}&page={1}&pageSize={2}&fromDate={3}&toDate={4}"; //Thông tin NN mua bán

        public const string _botToken = "5087420296:AAHVCnwFdVxdLFJsxgg8unqYN17rVWqFZfs";
    }
}


