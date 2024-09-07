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
                var body = $"StockCode={code}&UnitedId=-1&AuditedStatusId=-1&Unit=1000000000&IsNamDuongLich=false&PeriodType=QUY&SortTimeType=Time_ASC&__RequestVerificationToken=Qbhz8FkEYfdGbjJ3nhDb2ra4Rnbg9Ws3VuLoCvG4KMAlpDgMLzDnw-3psxQ1_A6Frt6tHVeCA5cxgc53FnMKsQt5ysN-WnvyynxAE7wQuLpA0qnuLpZT74NIoQ7rqji30";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Cookie", "language=vi-VN; Theme=Light; AnonymousNotification=; _pbjs_userid_consent_data=3524755945110770; isShowLogin=true; _cc_id=ebc6cdbd24cca9cce954e5a06cd7c5ba; panoramaId_expiry=1725723245184; panoramaId=fd5342b4d12fb3581e7ae9fa6c9c185ca02ce77fd625f98a7fe8fe514a59dedf; panoramaIdType=panoDevice; dable_uid=44708328.1725118445229; ASP.NET_SessionId=4idyib4u1rc0s2ncgenkk0v1; __RequestVerificationToken=-ny803vk8mbbdJEb8UqRDphtPszvmVh7sd8Z-zlAZ6OrPuJrdLl_QfKTxEp3GdMqL062vaH4yhn_JekmcJCq_M6emBlw6XdQTujORLjarXY1; _gid=GA1.2.2103867253.1725700047; __gads=ID=233a7d148667e1d0:T=1725118446:RT=1725700048:S=ALNI_Ma47tgzbY4avM5Kh3fy7OE-Nh4I2A; __gpi=UID=00000ee624024111:T=1725118446:RT=1725700048:S=ALNI_MYnrHXcXrZynGqtzL7IIHChNazPjg; __eoi=ID=c2eb48e8812aa43b:T=1725118446:RT=1725700048:S=AA-AfjYRP5qzLwziftRemNEtd4VP; vts_usr_lg=CF10EE1042B1BCB6027AC613017129F2DCDE8545E712713080E95B0AC741666FA27489BC791B84BA08DC9CC18FF0F847802D811FE0949DA4D28AEF98E8B4FBC94E8F80BB34FE53794A962CC481E48B0112D3E6416196CF5256C7C62400E7E31D5089E4BB7CC9CFC92FD8296C30716719; vst_usr_lg_token=G/kfULB87EO5p4684UUcqQ==; finance_viewedstock=NLG,HSG,; _ga=GA1.2.1298724755.1725118440; cto_bundle=c5jFT19FTzhUT3ZDSDh6QncxWlZ4REtCUnRXTnMzaWFjWENCamcwUiUyRnNweTRRQWJOQmFLSHNGS3RFN3pHVVY5c0ExWk9Ud2IzVVc0QU1INUJFYXJvNmNSUEtBUU90SGdzSVlvc3c1Zk54TnFNbk5HWDVwQlM5dHlPNTM1Y0RNZFZVenp4QVhCUlVNVGtsJTJGQSUyQmpZSHFkQmhPNEpiNFY5JTJCU0pYMFJwcEZLcWV6WEE3WSUzRA; cto_bidid=l5gv1F92JTJCeiUyRm1UbUtvcTROZkZKc0ZYV09RJTJGTDgwJTJGNDZzNTYlMkZ6c1I4NUY2OGhkTkw3bHFNeHExd3Y3eVhrN1U1Z3ZwaSUyQnBQR1k3MWo0cnRiZ20lMkJkUHBnZ1R1OWlWU2JhNVM3ZlpKM0NTNGlUU2FjJTNE; _ga_EXMM0DKVEX=GS1.1.1725700047.6.1.1725700114.60.0.0");
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
                var body = $"StockCode={code}&UnitedId=-1&AuditedStatusId=-1&Unit=1000000000&IsNamDuongLich=false&PeriodType=QUY&SortTimeType=Time_ASC&__RequestVerificationToken=Qbhz8FkEYfdGbjJ3nhDb2ra4Rnbg9Ws3VuLoCvG4KMAlpDgMLzDnw-3psxQ1_A6Frt6tHVeCA5cxgc53FnMKsQt5ysN-WnvyynxAE7wQuLpA0qnuLpZT74NIoQ7rqji30";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Cookie", "language=vi-VN; Theme=Light; AnonymousNotification=; _pbjs_userid_consent_data=3524755945110770; isShowLogin=true; _cc_id=ebc6cdbd24cca9cce954e5a06cd7c5ba; panoramaId_expiry=1725723245184; panoramaId=fd5342b4d12fb3581e7ae9fa6c9c185ca02ce77fd625f98a7fe8fe514a59dedf; panoramaIdType=panoDevice; dable_uid=44708328.1725118445229; ASP.NET_SessionId=4idyib4u1rc0s2ncgenkk0v1; __RequestVerificationToken=-ny803vk8mbbdJEb8UqRDphtPszvmVh7sd8Z-zlAZ6OrPuJrdLl_QfKTxEp3GdMqL062vaH4yhn_JekmcJCq_M6emBlw6XdQTujORLjarXY1; _gid=GA1.2.2103867253.1725700047; __gads=ID=233a7d148667e1d0:T=1725118446:RT=1725700048:S=ALNI_Ma47tgzbY4avM5Kh3fy7OE-Nh4I2A; __gpi=UID=00000ee624024111:T=1725118446:RT=1725700048:S=ALNI_MYnrHXcXrZynGqtzL7IIHChNazPjg; __eoi=ID=c2eb48e8812aa43b:T=1725118446:RT=1725700048:S=AA-AfjYRP5qzLwziftRemNEtd4VP; vts_usr_lg=CF10EE1042B1BCB6027AC613017129F2DCDE8545E712713080E95B0AC741666FA27489BC791B84BA08DC9CC18FF0F847802D811FE0949DA4D28AEF98E8B4FBC94E8F80BB34FE53794A962CC481E48B0112D3E6416196CF5256C7C62400E7E31D5089E4BB7CC9CFC92FD8296C30716719; vst_usr_lg_token=G/kfULB87EO5p4684UUcqQ==; finance_viewedstock=NLG,HSG,; _ga=GA1.2.1298724755.1725118440; cto_bundle=c5jFT19FTzhUT3ZDSDh6QncxWlZ4REtCUnRXTnMzaWFjWENCamcwUiUyRnNweTRRQWJOQmFLSHNGS3RFN3pHVVY5c0ExWk9Ud2IzVVc0QU1INUJFYXJvNmNSUEtBUU90SGdzSVlvc3c1Zk54TnFNbk5HWDVwQlM5dHlPNTM1Y0RNZFZVenp4QVhCUlVNVGtsJTJGQSUyQmpZSHFkQmhPNEpiNFY5JTJCU0pYMFJwcEZLcWV6WEE3WSUzRA; cto_bidid=l5gv1F92JTJCeiUyRm1UbUtvcTROZkZKc0ZYV09RJTJGTDgwJTJGNDZzNTYlMkZ6c1I4NUY2OGhkTkw3bHFNeHExd3Y3eVhrN1U1Z3ZwaSUyQnBQR1k3MWo0cnRiZ20lMkJkUHBnZ1R1OWlWU2JhNVM3ZlpKM0NTNGlUU2FjJTNE; _ga_EXMM0DKVEX=GS1.1.1725700047.6.1.1725700114.60.0.0");
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
