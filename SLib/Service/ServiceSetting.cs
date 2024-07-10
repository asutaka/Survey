namespace SLib.Service
{
    public static class ServiceSetting
    {
        public const string _stockExchange = "https://bgapidatafeed.vps.com.vn/getlistckindex/{0}";
        public const string _stockData = "https://iboard-api.ssi.com.vn/statistics/charts/history?symbol={0}&resolution={1}&from={2}&to={3}";
        public const string _companyInfo = "https://iboard-api.ssi.com.vn/statistics/company/company-profile?symbol={0}&language=vn"; //Thông tin cty, nhóm ngành
        public const string _shareHolder = "https://iboard-api.ssi.com.vn/statistics/company/shareholders?symbol={0}&language=vn&page=1&pageSize=100"; //Thông tin cổ đông
        public const string _foreignBuySell = "https://iboard-api.ssi.com.vn/statistics/company/stock-price?symbol={0}&page={1}&pageSize={2}&fromDate={3}&toDate={4}"; //Thông tin NN mua bán
        public const string _financeInfo = "https://iboard-api.ssi.com.vn/statistics/company/financial-indicator?symbol={0}&page=1&pageSize=1000";//Thông tin tài chính

        public const string _tudoanhHNX = "https://owa.hnx.vn/ftp//PORTALNEW/HEADER_IMAGES/{0}/{0}_Chi_tiet_gd_tu_doanh_theo_ma_ck%20{1}.pdf";

        public const string _botToken = "7422438658:AAEPzAwq-5rA-5dLEFRpPrdOBt9yMfnkBxA";
    }
}


