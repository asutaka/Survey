using HttpClientToCurl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockLib.Model;
using System.Text;

namespace StockLib.Service
{
    public interface IAPIService
    {
        Task<ReportDataIDResponse> VietStock_CDKT_GetListReportData(string code);
        Task<ReportDataIDResponse> VietStock_KQKD_GetListReportData(string code);
        Task<ReportTempIDResponse> VietStock_CSTC_GetListTempID(string code);
        Task<ReportDataDetailValue_BCTTResponse> VietStock_GetReportDataDetailValue_KQKD_ByReportDataIds(string body);
        Task<ReportDataDetailValue_BCTTResponse> VietStock_GetReportDataDetailValue_CDKT_ByReportDataIds(string body);
        Task<TempDetailValue_CSTCResponse> VietStock_GetFinanceIndexDataValue_CSTC_ByListTerms(string body);
        Task<IEnumerable<BCTCAPIResponse>> VietStock_GetDanhSachBCTC(string code, int page);
    }
    public partial class APIService : IAPIService
    {
        private readonly ILogger<APIService> _logger;
        private readonly IHttpClientFactory _client;
        public APIService(ILogger<APIService> logger,
                        IHttpClientFactory httpClientFactory) 
        {
            _logger = logger;
            _client = httpClientFactory;
        }

        public async Task<ReportDataIDResponse> VietStock_CDKT_GetListReportData(string code)
        {
            var url = "https://finance.vietstock.vn/data/CDKT_GetListReportData";
            return await VietStock_GetListReportData(code, url);
        }

        public async Task<ReportDataDetailValue_BCTTResponse> VietStock_GetReportDataDetailValue_CDKT_ByReportDataIds(string body)
        {
            var url = "https://finance.vietstock.vn/data/GetReportDataDetailValueByReportDataIds";
            return await VietStock_GetReportDataDetailValue(body, url);
        }

        public async Task<ReportDataIDResponse> VietStock_KQKD_GetListReportData(string code)
        {
            var url = "https://finance.vietstock.vn/data/KQKD_GetListReportData";
            return await VietStock_GetListReportData(code, url);
        }

        public async Task<ReportDataDetailValue_BCTTResponse> VietStock_GetReportDataDetailValue_KQKD_ByReportDataIds(string body)
        {
            var url = "https://finance.vietstock.vn/data/GetReportDataDetailValue_KQKD_ByReportDataIds";
            return await VietStock_GetReportDataDetailValue(body, url);
        }

        public async Task<ReportTempIDResponse> VietStock_CSTC_GetListTempID(string code)
        {
            var url = "https://finance.vietstock.vn/data/CSTC_GetListTerms";
            return await VietStock_GetListTempID(code, url);
        }

        public async Task<TempDetailValue_CSTCResponse> VietStock_GetFinanceIndexDataValue_CSTC_ByListTerms(string body)
        {
            var url = "https://finance.vietstock.vn/data/GetFinanceIndexDataValue_CSTC_ByListTerms";
            return await GetFinanceIndexDataValue(body, url);
        }

        public async Task<IEnumerable<BCTCAPIResponse>> VietStock_GetDanhSachBCTC(string code, int page)
        {
            var body = $"code={code}&page={page}&type=1&__RequestVerificationToken=R9DyxHzZguFIy3laKfCYGNRl1FpfNH31U6TY1JG85XqHmc8hCcIPbPF74Q0n9YQua9-h3H9nyC3H31cwbcscI6Udw0udfcYmfl1_Zk9Xzco1";
            try
            {
                var url = "https://finance.vietstock.vn/data/getdocument";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                //requestMessage.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                requestMessage.Headers.Add("Cookie", "language=vi-VN; ASP.NET_SessionId=arn5rig5dxbmmjoizukepdtp; __RequestVerificationToken=lsueRAFEInzD1hnxKUdgW2KwoJG9BTcIbmeErlT-ZbsnP-s9bR_xm25X1mXPVAj19x9vzoY6uLnwZW889qjTsK-YscWe_4RZQo5ErQDjJBE1; Theme=Light; AnonymousNotification=; finance_viewedstock=ACB,");
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                var responseMessage = await client.SendAsync(requestMessage);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<IEnumerable<BCTCAPIResponse>>(resultArray);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VietStock_GetDanhSachBCTC|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}

