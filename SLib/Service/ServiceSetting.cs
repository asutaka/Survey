﻿namespace SLib.Service
{
    public static class ServiceSetting
    {
        public const string _stockExchange = "https://bgapidatafeed.vps.com.vn/getlistckindex/{0}";
        public const string _stockData = "https://iboard-api.ssi.com.vn/statistics/charts/history?symbol={0}&resolution={1}&from={2}&to={3}";
        public const string _companyInfo_ssi = "https://iboard-api.ssi.com.vn/statistics/company/company-profile?symbol={0}&language=vn"; //Thông tin cty, nhóm ngành
        public const string _shareHolder_ssi = "https://iboard-api.ssi.com.vn/statistics/company/shareholders?symbol={0}&language=vn&page=1&pageSize=100"; //Thông tin cổ đông
        public const string _foreignBuySell_ssi = "https://iboard-api.ssi.com.vn/statistics/company/stock-price?symbol={0}&page={1}&pageSize={2}&fromDate={3}&toDate={4}"; //Thông tin NN mua bán
        public const string _financeInfo_ssi = "https://iboard-api.ssi.com.vn/statistics/company/financial-indicator?symbol={0}&page=1&pageSize=1000";//Thông tin tài chính

        public const string _tudoanhHNX = "https://owa.hnx.vn/ftp//PORTALNEW/HEADER_IMAGES/{0}/{0}_Chi_tiet_gd_tu_doanh_theo_ma_ck%20{1}.pdf";

        public const string _botToken = "7422438658:AAEPzAwq-5rA-5dLEFRpPrdOBt9yMfnkBxA";

        public const string _giaodichNN_24hMoney = "https://api-finance-t19.24hmoney.vn/v2/web/indices/foreign-trading-all-stock-by-time?code={0}&type={1}";//Giao dịch nước ngoài
        public const string _giaodichTuDoanh_24hMoney = "https://api-finance-t19.24hmoney.vn/v2/web/indices/proprietary-trading-all-stock-by-time?code={0}&type=today";//Giao dịch tự doanh
        public const string _nhomNganh_24hMoney = "https://api-finance-t19.24hmoney.vn/v2/ios/company-group/all-level-with-summary?type={0}";//Dữ liệu nhóm ngành
        public const string _maTheoNganh_24hMoney = "https://api-finance-t19.24hmoney.vn/v2/ios/stock-recommend/business?group_id={0}&page=1&per_page=500";//Lấy danh sách Mã CK theo nhóm ngành
        public const string _maTheoChiBao_24hMoney = "https://api-finance-t19.24hmoney.vn/v2/web/indices/technical-signal-filter?sort=asc&page=1&per_page=200";//Lấy danh sách Mã CK theo chỉ báo
        public const string _keHoachThucHien_24hMoney = "https://api-finance-t19.24hmoney.vn/v1/ios/company/plan-all?symbol={0}";//Lấy kế hoạch kinh doanh của cty
        public const string _loinhuan_24hMoney = "https://api-finance-t19.24hmoney.vn/v1/ios/company/financial-graph?symbol={0}&graph_type=4";//Thống kê lợi nhuận doanh nghiệp
    }
}

