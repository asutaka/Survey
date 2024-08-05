using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using iTextSharp.text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockLib.Model;
using StockLib.Service.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public interface IAPIService
    {
        Task<Stream> BCTCRead(string path);
        Task<ReportNormResponse> VietStock_GetListReportNorm_BCTT_ByStockCode(string code);
        Task<ReportDataIDResponse> VietStock_BCTT_GetListReportData(string code);
        Task<ReportDataDetailValue_BCTTResponse> VietStock_GetReportDataDetailValue_BCTT_ByReportDataIds(string body);
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
        public async Task<Stream> BCTCRead(string path)
        {
            try
            {
                var stream = await GetFile(path);
                if (stream is null) return null;
                var upload = await UploadFile(stream);
                if (upload is null) return null;
                var ocr = await OCRFile(upload);
                if (ocr is null) return null;
                var count = 1;
                BCTCStatusResponse status = null;
                do
                {
                    count++;
                    status = await GetStatus(ocr);
                    if (status != null)
                    {
                        if ("done".Equals(status.status, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }

                    if (count >= 100)
                    {
                        break;
                    }

                    Thread.Sleep(5000);
                }
                while (true);

                if (status is null)
                    return null;

                if ("done".Equals(status.status, StringComparison.OrdinalIgnoreCase))
                {
                    var streamResult = await DownloadFile(status);
                    return streamResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.BCTCRead|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        public async Task<ReportNormResponse> VietStock_GetListReportNorm_BCTT_ByStockCode(string code)
        {
            try
            {
                var body = $"stockCode={code}&__RequestVerificationToken=A53PUyn5CX2ETDYUQTE8oIJJZGJyLjsqCFk563WNo1JN-VDO1ttVWrqr-8td8nJbjD-UzwebEFkY0uXVBD5U8v2TibI-VNGIaZMVTSucyCY1";
                var url = "https://finance.vietstock.vn/data/GetListReportNorm_BCTT_ByStockCode";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Cookie", "ASP.NET_SessionId=nlehv103ci5raqevopu2cysl; __RequestVerificationToken=Mmg6CGomYCXe4BZkIkoJRLvUmhZ99CUb4coeUeaymZCN9xYHumQFNeWQBeu2TlJriagiaA-3vecniiRfEVRWQBIsYA-Gi-PysFMLQ8FjpXY1; language=vi-VN; Theme=Light; AnonymousNotification=; finance_viewedstock=HDG,; ASP.NET_SessionId=xay54kxdkpja0bmirimxjojd; __RequestVerificationToken=N0fSL-m3glCiEom91SsobZ0JEj-d5TLOqbtH5eJ2JsycfriP4L-d4OkDavPx50uXNbBv6KNsMuStIKsS-buiS2Id5SAHumoVzbuUJh_RE2c1; language=vi-VN");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Mobile Safari/537.36");
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                var responseMessage = await client.SendAsync(requestMessage);
                if(responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseStr = await responseMessage.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<ReportNormResponse>(responseStr);
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VietStock_GetListReportNorm_BCTT_ByStockCode|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<ReportDataIDResponse> VietStock_BCTT_GetListReportData(string code)
        {
            try
            {
                var body = $"StockCode={code}&UnitedId=-1&AuditedStatusId=-1&Unit=1000000000&IsNamDuongLich=false&PeriodType=QUY&SortTimeType=Time_ASC&__RequestVerificationToken=A53PUyn5CX2ETDYUQTE8oIJJZGJyLjsqCFk563WNo1JN-VDO1ttVWrqr-8td8nJbjD-UzwebEFkY0uXVBD5U8v2TibI-VNGIaZMVTSucyCY1";
                var url = "https://finance.vietstock.vn/data/BCTT_GetListReportData";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Cookie", "ASP.NET_SessionId=nlehv103ci5raqevopu2cysl; __RequestVerificationToken=Mmg6CGomYCXe4BZkIkoJRLvUmhZ99CUb4coeUeaymZCN9xYHumQFNeWQBeu2TlJriagiaA-3vecniiRfEVRWQBIsYA-Gi-PysFMLQ8FjpXY1; language=vi-VN; Theme=Light; AnonymousNotification=; finance_viewedstock=HDG,; ASP.NET_SessionId=xay54kxdkpja0bmirimxjojd; __RequestVerificationToken=N0fSL-m3glCiEom91SsobZ0JEj-d5TLOqbtH5eJ2JsycfriP4L-d4OkDavPx50uXNbBv6KNsMuStIKsS-buiS2Id5SAHumoVzbuUJh_RE2c1; language=vi-VN");
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
                _logger.LogError($"APIService.VietStock_BCTT_GetListReportData|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<ReportDataDetailValue_BCTTResponse> VietStock_GetReportDataDetailValue_BCTT_ByReportDataIds(string body)
        {
            try
            {
                
                var url = "https://finance.vietstock.vn/data/GetReportDataDetailValue_BCTT_ByReportDataIds";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Cookie", "ASP.NET_SessionId=nlehv103ci5raqevopu2cysl; __RequestVerificationToken=Mmg6CGomYCXe4BZkIkoJRLvUmhZ99CUb4coeUeaymZCN9xYHumQFNeWQBeu2TlJriagiaA-3vecniiRfEVRWQBIsYA-Gi-PysFMLQ8FjpXY1; language=vi-VN; Theme=Light; AnonymousNotification=; finance_viewedstock=HDG,; ASP.NET_SessionId=xay54kxdkpja0bmirimxjojd; __RequestVerificationToken=N0fSL-m3glCiEom91SsobZ0JEj-d5TLOqbtH5eJ2JsycfriP4L-d4OkDavPx50uXNbBv6KNsMuStIKsS-buiS2Id5SAHumoVzbuUJh_RE2c1; language=vi-VN");
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
                _logger.LogError($"APIService.VietStock_GetReportDataDetailValue_BCTT_ByReportDataIds|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}

