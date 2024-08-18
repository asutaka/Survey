using HttpClientToCurl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockLib.Model;
using System.Text;

namespace StockLib.Service
{
    public partial class APIService
    {
        private async Task<ReportDataIDResponse> VietStock_GetListReportData(string code, string url)
        {
            try
            {
                var body = $"StockCode={code}&UnitedId=-1&AuditedStatusId=-1&Unit=1000000000&IsNamDuongLich=false&PeriodType=QUY&SortTimeType=Time_ASC&__RequestVerificationToken=2zq3UJ1lr0dvtlqSWV0KBxMcxb6DdeiHkp24FLqLP5PxRo8mdOFWdmZ1pVp6TeSF6okBe49tweByhCY3tYxHcXd-K5whsT2jgg4k4jf0fEtAKcLa1xG677l_vtWo3g4K0";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Cookie", "ASP.NET_SessionId=kez23nkspuoomciouahd1xqp; __RequestVerificationToken=5t0qgD3M2IWZKLXukNsWaFE2ZCWl_cKVOn2SDHUDDw6NIEfBM1FC1HWEnrE9BzsrKeZrRWbGyYItV21WS4E6t-CTsKZqRvQIv6Ma5qAegwU1; language=vi-VN; _ga=GA1.1.1323687995.1720524498; _ga_EXMM0DKVEX=GS1.1.1720524497.1.0.1720524497.60.0.0; Theme=Light; vts_usr_lg=BE054785A7D153883D1375FA0F60E70818133565FC72863385609A64216746D9778737512F5F732882E1DEC3C7BE872FF2CA637FAEC20983E39487993FFAC05F808C0F33EB77D6330B290268C6485AC281D88D85F29860D4883C2C30A803B3ABBF2053F281E0B1D7F78E55928B856D012D6CDDC27F368D39996D5B02F8C35406; finance_viewedstock=TCH,FIR,HID,TPB,HDG,; ASP.NET_SessionId=gk4jgohvpkgtzehrxterafcb; __RequestVerificationToken=ZDxf13qxQFoRu5y2IzeiH7tyqgweTrnCuyeZsWQWYJvFmD9TQFao5FCntC0-8cPI4CBZCLrJk9LFfKMCUKmyDiM90eEaNU83ss05J27t3Bk1; language=vi-VN");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Mobile Safari/537.36");
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseStr = await responseMessage.Content.ReadAsStringAsync();
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
                var body = $"StockCode={code}&UnitedId=-1&AuditedStatusId=-1&Unit=1000000000&IsNamDuongLich=false&PeriodType=QUY&SortTimeType=Time_ASC&__RequestVerificationToken=2zq3UJ1lr0dvtlqSWV0KBxMcxb6DdeiHkp24FLqLP5PxRo8mdOFWdmZ1pVp6TeSF6okBe49tweByhCY3tYxHcXd-K5whsT2jgg4k4jf0fEtAKcLa1xG677l_vtWo3g4K0";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Cookie", "ASP.NET_SessionId=kez23nkspuoomciouahd1xqp; __RequestVerificationToken=5t0qgD3M2IWZKLXukNsWaFE2ZCWl_cKVOn2SDHUDDw6NIEfBM1FC1HWEnrE9BzsrKeZrRWbGyYItV21WS4E6t-CTsKZqRvQIv6Ma5qAegwU1; language=vi-VN; _ga=GA1.1.1323687995.1720524498; _ga_EXMM0DKVEX=GS1.1.1720524497.1.0.1720524497.60.0.0; Theme=Light; vts_usr_lg=BE054785A7D153883D1375FA0F60E70818133565FC72863385609A64216746D9778737512F5F732882E1DEC3C7BE872FF2CA637FAEC20983E39487993FFAC05F808C0F33EB77D6330B290268C6485AC281D88D85F29860D4883C2C30A803B3ABBF2053F281E0B1D7F78E55928B856D012D6CDDC27F368D39996D5B02F8C35406; finance_viewedstock=TCH,FIR,HID,TPB,HDG,; ASP.NET_SessionId=gk4jgohvpkgtzehrxterafcb; __RequestVerificationToken=ZDxf13qxQFoRu5y2IzeiH7tyqgweTrnCuyeZsWQWYJvFmD9TQFao5FCntC0-8cPI4CBZCLrJk9LFfKMCUKmyDiM90eEaNU83ss05J27t3Bk1; language=vi-VN");
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
                client.DefaultRequestHeaders.Add("Cookie", "Cookie: ASP.NET_SessionId=kez23nkspuoomciouahd1xqp; __RequestVerificationToken=5t0qgD3M2IWZKLXukNsWaFE2ZCWl_cKVOn2SDHUDDw6NIEfBM1FC1HWEnrE9BzsrKeZrRWbGyYItV21WS4E6t-CTsKZqRvQIv6Ma5qAegwU1; language=vi-VN; _ga=GA1.1.1323687995.1720524498; _ga_EXMM0DKVEX=GS1.1.1720524497.1.0.1720524497.60.0.0; Theme=Light; vts_usr_lg=BE054785A7D153883D1375FA0F60E70818133565FC72863385609A64216746D9778737512F5F732882E1DEC3C7BE872FF2CA637FAEC20983E39487993FFAC05F808C0F33EB77D6330B290268C6485AC281D88D85F29860D4883C2C30A803B3ABBF2053F281E0B1D7F78E55928B856D012D6CDDC27F368D39996D5B02F8C35406; finance_viewedstock=HDG,; language=vi-VN");
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
                client.DefaultRequestHeaders.Add("Cookie", "Cookie: ASP.NET_SessionId=kez23nkspuoomciouahd1xqp; __RequestVerificationToken=5t0qgD3M2IWZKLXukNsWaFE2ZCWl_cKVOn2SDHUDDw6NIEfBM1FC1HWEnrE9BzsrKeZrRWbGyYItV21WS4E6t-CTsKZqRvQIv6Ma5qAegwU1; language=vi-VN; _ga=GA1.1.1323687995.1720524498; _ga_EXMM0DKVEX=GS1.1.1720524497.1.0.1720524497.60.0.0; Theme=Light; vts_usr_lg=BE054785A7D153883D1375FA0F60E70818133565FC72863385609A64216746D9778737512F5F732882E1DEC3C7BE872FF2CA637FAEC20983E39487993FFAC05F808C0F33EB77D6330B290268C6485AC281D88D85F29860D4883C2C30A803B3ABBF2053F281E0B1D7F78E55928B856D012D6CDDC27F368D39996D5B02F8C35406; finance_viewedstock=HDG,; language=vi-VN");
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
