using HttpClientToCurl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockLib.Model;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class APIService
    {
        private async Task<ReportDataIDResponse> VietStock_GetListReportData(string code, string url)
        {
            try
            {
                var body = $"StockCode={code}&UnitedId=-1&AuditedStatusId=-1&Unit=1000000000&IsNamDuongLich=false&PeriodType=QUY&SortTimeType=Time_ASC&__RequestVerificationToken={StaticVal._VietStock_Token}";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("Cookie", StaticVal._VietStock_Cookie);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Mobile Safari/537.36");
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseStr = await responseMessage.Content.ReadAsStringAsync();
                    if (responseStr.Contains("<!DOCTYPE"))
                        return null;

                    var responseModel = JsonConvert.DeserializeObject<ReportDataIDResponse>(responseStr);
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VietStock_GetListReportData|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<ReportTempIDResponse> VietStock_GetListTempID(string code, string url)
        {
            try
            {
                var body = $"StockCode={code}&UnitedId=-1&AuditedStatusId=-1&Unit=1000000000&IsNamDuongLich=false&PeriodType=QUY&SortTimeType=Time_ASC&__RequestVerificationToken={StaticVal._VietStock_Token}";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("Cookie", StaticVal._VietStock_Cookie);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Mobile Safari/537.36");
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseStr = await responseMessage.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<ReportTempIDResponse>(responseStr);
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VietStock_GetListTempID|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<ReportDataDetailValue_BCTTResponse> VietStock_GetReportDataDetailValue(string body, string url)
        {
            try
            {

                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("Cookie", StaticVal._VietStock_Cookie);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Mobile Safari/537.36");
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseStr = await responseMessage.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<ReportDataDetailValue_BCTTResponse>(responseStr);
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VietStock_GetReportDataDetailValue|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        
        private async Task<TempDetailValue_CSTCResponse> GetFinanceIndexDataValue(string body, string url)
        {
            try
            {

                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("Cookie", StaticVal._VietStock_Cookie);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Mobile Safari/537.36");
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseStr = await responseMessage.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<TempDetailValue_CSTCResponse>(responseStr);
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.GetFinanceIndexDataValue|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<List<Money24h_PTKTResponse>> Money24h_GetMaTheoChiBao(string chibao)
        {
            var lOutput = new List<Money24h_PTKTResponse>();
            try
            {
                var body = "{\"floor_codes\":[\"10\",\"11\",\"02\",\"03\"],\"group_ids\":[\"0001\",\"1000\",\"2000\",\"3000\",\"4000\",\"5000\",\"6000\",\"7000\",\"8000\",\"8301\",\"9000\"],\"signals\":[{\"" + chibao + "\":\"up\"}]}";
                var url = "https://api-finance-t19.24hmoney.vn/v2/web/indices/technical-signal-filter?sort=asc&page=1&per_page=50";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");

                var responseMessage = await client.SendAsync(requestMessage);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<Money24h_PTKT_LV1Response>(resultArray);
                if (responseModel.status == 200
                    && responseModel.data.total_symbol > 0
                    && responseModel.data.data.Any())
                {
                    return responseModel.data.data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Money24h_GetMaTheoChiBao|EXCEPTION| {ex.Message}");
            }
            return lOutput;
        }
    }
}
