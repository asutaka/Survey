namespace SurveyStock.Service
{
    public static class ServiceSetting
    {
        public const string _stockExchange = "https://bgapidatafeed.vps.com.vn/getlistckindex/{0}";
        public const string _companyInfo = "https://iboard-api.ssi.com.vn/statistics/company/company-profile?symbol={0}&language=vn"; //Thông tin cty, nhóm ngành
        public const string _shareHolder = "https://iboard-api.ssi.com.vn/statistics/company/shareholders?symbol={0}&language=vn&page=1&pageSize=100"; //Thông tin cổ đông
    }
}


