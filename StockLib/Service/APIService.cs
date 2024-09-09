using HtmlAgilityPack;
using HttpClientToCurl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using StockLib.Model;
using StockLib.Service.Settings;
using StockLib.Utils;
using System.Text;
using System.Xml;

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
        Task<Stream> GetChartImage(string body);

        Task<List<Money24h_PTKTResponse>> Money24h_GetMaTheoChiBao_MA20();
        Task<List<Money24h_PTKTResponse>> Money24h_GetMaTheoChiBao_52W();
        Task<Money24h_NhomNganhResponse> Money24h_GetNhomNganh(EMoney24hTimeType type);
        Task<List<Money24h_ForeignResponse>> Money24h_GetForeign(EMoney24hExchangeMode mode, EMoney24hTimeType type);
        Task<List<Money24h_KeHoach_Data>> Money24h_GetKeHoach(string code);


        Task<List<Quote>> SSI_GetDataStock(string code);
        Task<List<SSI_PEDetail>> SSI_GetFinance(string code);

        Task<Stream> TuDoanhHNX(EHnxExchange mode, DateTime dt);
        Task<Stream> TuDoanhHSX(DateTime dt);
        Task<Stream> TongCucThongKe(DateTime dt);
        Task<GOV_HaiQuanResponse> TongCucHaiQuan();
        Task<Stream> TongCucHaiQuan(string url);
        Task<Stream> TongCucThongKeTest(DateTime dt);
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

        #region Vietstock
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
            var body = $"code={code}&page={page}&type=1&__RequestVerificationToken=9a3VgzLV-OF13vpUO4ZZZaYuc9nMSuYJluPAgu7mU-Rpn-c0c8j4tKvFHA1btqPWUexg4pMMYqLIU3bggz-O0xZkdR9thlg1ZwqGcU4ObPhTz8sh97k6mpGzwmeSdqHQ0";
            try
            {
                var url = "https://finance.vietstock.vn/data/getdocument";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                //requestMessage.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                requestMessage.Headers.Add("Cookie", "ASP.NET_SessionId=kez23nkspuoomciouahd1xqp; __RequestVerificationToken=5t0qgD3M2IWZKLXukNsWaFE2ZCWl_cKVOn2SDHUDDw6NIEfBM1FC1HWEnrE9BzsrKeZrRWbGyYItV21WS4E6t-CTsKZqRvQIv6Ma5qAegwU1; language=vi-VN; _ga=GA1.1.1323687995.1720524498; _ga_EXMM0DKVEX=GS1.1.1720524497.1.0.1720524497.60.0.0; vts_usr_lg=A48AA659415FEE16F7CD0976F49923629A486E7AD4073A0F7F92268AEC798D2599F993737255E9990209E28582ABC797C68C46C2209B505875B2542FB92A23DCEFB76C9610DA504D7C120024CAB560DA51EC06B2D17034BFA7F517529B3FF3340438A76004E762194D4CAC1C45600B90CF3FF9475AB984756A4F22DC52765B59; finance_viewedstock=ACB,; Theme=Light");
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
        #endregion

        public async Task<Stream> GetChartImage(string body)
        {
            try
            {
                var url = ServiceSetting._chartLocal;
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                //requestMessage.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");

                var responseMessage = await client.SendAsync(requestMessage);
                var result = await responseMessage.Content.ReadAsStreamAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.GetChartImage|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        #region 24H Money
        public async Task<List<Money24h_PTKTResponse>> Money24h_GetMaTheoChiBao_MA20()
        {
            return await Money24h_GetMaTheoChiBao("ma20");
        }

        public async Task<List<Money24h_PTKTResponse>> Money24h_GetMaTheoChiBao_52W()
        {
            return await Money24h_GetMaTheoChiBao("break_1y");
        }

        public async Task<Money24h_NhomNganhResponse> Money24h_GetNhomNganh(EMoney24hTimeType type)
        {
            try
            {
                var urlBase = "https://api-finance-t19.24hmoney.vn/v2/ios/company-group/all-level-with-summary?type={0}";
                var url = string.Format(urlBase, type.GetDisplayName());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var result = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<Money24h_NhomNganhResponse>(result);
                if (responseModel.status == 200)
                {
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Money24h_GetNhomNganh|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<Money24h_ForeignResponse>> Money24h_GetForeign(EMoney24hExchangeMode mode, EMoney24hTimeType type)
        {
            var lOutput = new List<Money24h_ForeignResponse>();
            try
            {
                var urlBase = "https://api-finance-t19.24hmoney.vn/v2/web/indices/foreign-trading-all-stock-by-time?code={0}&type={1}";
                var url = string.Format(urlBase, mode.GetDisplayName(), type.GetDisplayName());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<Money24h_ForeignAPIResponse>(resultArray);
                if (responseModel.status == 200
                    && responseModel.data.data.Any())
                {
                    var date = responseModel.data.from_date.ToDateTime("dd/MM/yyyy");
                    if (date.Day == DateTime.Now.Day)
                    {
                        return responseModel.data.data.Where(x => x.symbol.Length == 3).OrderByDescending(x => x.net_val).Select((x, index) => new Money24h_ForeignResponse
                        {
                            no = index + 1,
                            d = new DateTimeOffset(date, TimeSpan.FromHours(0)).ToUnixTimeSeconds(),
                            s = x.symbol,
                            sell_qtty = x.sell_qtty,
                            sell_val = x.sell_val,
                            buy_qtty = x.buy_qtty,
                            buy_val = x.buy_val,
                            net_val = x.net_val,
                            t = DateTimeOffset.Now.ToUnixTimeSeconds()
                        }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Money24h_GetForeign|EXCEPTION| {ex.Message}");
            }
            return lOutput;
        }

        public async Task<List<Money24h_KeHoach_Data>> Money24h_GetKeHoach(string code)
        {
            try
            {
                var url = $"https://api-finance-t19.24hmoney.vn/v1/ios/company/plan-all?symbol={code}";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var result = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<Money24h_KeHoach>(result);
                if (responseModel.status == 200)
                {
                    return responseModel.data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Money24h_GetKeHoach|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        #endregion

        #region SSI
        public async Task<List<Quote>> SSI_GetDataStock(string code)
        {
            var lOutput = new List<Quote>();
            var urlBase = "https://iboard-api.ssi.com.vn/statistics/charts/history?symbol={0}&resolution={1}&from={2}&to={3}";
            try
            {
                var url = string.Format(urlBase, code, "1D", DateTimeOffset.Now.AddYears(-2).ToUnixTimeSeconds(), DateTimeOffset.Now.ToUnixTimeSeconds());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<SSI_DataTradingResponse>(resultArray);
                if (responseModel.data.t.Any())
                {
                    var count = responseModel.data.t.Count();
                    for (int i = 0; i < count; i++)
                    {
                        lOutput.Add(new Quote
                        {
                            Date = responseModel.data.t.ElementAt(i).UnixTimeStampToDateTime(),
                            Open = responseModel.data.o.ElementAt(i),
                            Close = responseModel.data.c.ElementAt(i),
                            High = responseModel.data.h.ElementAt(i),
                            Low = responseModel.data.l.ElementAt(i),
                            Volume = responseModel.data.v.ElementAt(i)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.SSI_GetDataStock|EXCEPTION| {ex.Message}");
            }
            return lOutput;
        }

        public async Task<List<SSI_PEDetail>> SSI_GetFinance(string code)
        {
            var url = $"https://iboard-api.ssi.com.vn/statistics/company/financial-indicator?symbol={code}&page=1&pageSize=1000";
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<SSI_PEResponse>(responseMessageStr);
                return responseModel.data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.SSI_GetFinance|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        #endregion

        public async Task<Stream> TuDoanhHNX(EHnxExchange mode, DateTime dt)
        {
            try
            {
                var urlBase = "https://owa.hnx.vn/ftp//PORTALNEW/HEADER_IMAGES/{0}/{0}_Chi_tiet_gd_tu_doanh_theo_ma_ck%20{1}.pdf";
                var strDate = $"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}";
                var url = string.Format(urlBase, strDate, mode.ToString());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                return await responseMessage.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.TuDoanhHNX|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> TuDoanhHSX(DateTime dt)
        {
            try
            {
                //LV1
                var link = string.Empty;
                var url = "https://www.hsx.vn";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"body\"]/div[2]/div[1]/div[2]/div[1]/div/div[2]/div") as IEnumerable<HtmlNode>;
                foreach (HtmlNode node in nodes.ElementAt(0).ChildNodes)
                {
                    if (string.IsNullOrWhiteSpace(node.InnerText))
                        continue;

                    var document = new HtmlDocument();
                    document.LoadHtml(node.InnerHtml);
                    var lNode = document.DocumentNode.SelectNodes("//a");
                    foreach (var item in lNode)
                    {
                        var tagA = document.DocumentNode.SelectSingleNode("//a");
                        var title = tagA.Attributes["title"].Value;
                        if (!(title.Contains("giao dịch tự doanh")
                            && title.Contains($"{dt.Day.To2Digit()}/{dt.Month.To2Digit()}/{dt.Year}")))
                            continue;

                        link = tagA.Attributes["href"].Value;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(link))
                    return null;

                //LV2
                var clientDetail = new HttpClient { BaseAddress = new Uri($"{url}{link.Replace("ViewArticle", "GetRelatedFiles")}?rows=30&page=1") };
                responseMessage = await clientDetail.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var content = await responseMessage.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<HSXTudoanhModel>(content);
                var lastID = model.rows?.FirstOrDefault()?.cell?.FirstOrDefault();
                //LV3
                var clientDownload = new HttpClient { BaseAddress = new Uri($"{url}/Modules/CMS/Web/DownloadFile?id={lastID}") };
                responseMessage = await clientDownload.GetAsync("", HttpCompletionOption.ResponseContentRead);
                return await responseMessage.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.TuDoanhHSX|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> TongCucThongKe(DateTime dt)
        {
            try
            {
                //LV1
                //var url = "https://www.gso.gov.vn/bao-cao-tinh-hinh-kinh-te-xa-hoi-hang-thang/?paged=2";
                var url = "https://www.gso.gov.vn/bao-cao-tinh-hinh-kinh-te-xa-hoi-hang-thang/";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var linkedPages = doc.DocumentNode.Descendants("a")
                                                  .Select(a => a.GetAttributeValue("href", null))
                                                  .Where(u => !string.IsNullOrWhiteSpace(u))
                                                  .Where(x => x.Contains(dt.Year.ToString()) && x.Contains(dt.Month.To2Digit().ToString()));

                var link = linkedPages.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(link))
                    return null;
                //LV2
                var clientDetail = new HttpClient { BaseAddress = new Uri(link) };
                responseMessage = await clientDetail.GetAsync("", HttpCompletionOption.ResponseContentRead);
                html = await responseMessage.Content.ReadAsStringAsync();
                doc.LoadHtml(html);
                linkedPages = doc.DocumentNode.Descendants("a")
                                                  .Select(a => a.GetAttributeValue("href", null))
                                                  .Where(u => !string.IsNullOrWhiteSpace(u))
                                                  .Where(x => x.Contains(dt.Year.ToString()) && x.Contains(dt.Month.To2Digit().ToString()) && x.Contains(".xlsx"));
                link = linkedPages.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(link))
                    return null;
                //LV3
                var clientDownload = new HttpClient { BaseAddress = new Uri(link) };
                responseMessage = await clientDownload.GetAsync("", HttpCompletionOption.ResponseContentRead);
                return await responseMessage.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.TongCucThongKe|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<GOV_HaiQuanResponse> TongCucHaiQuan()
        {
            try
            {
                var url = "https://www.customs.gov.vn/bridge?url=/customs/api/GetTKHQInfo";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                var body = "{\"skip\":0,\"take\":5,\"ky\":\"\",\"textSearch\":\"\",\"the_loai\":\"0\",\"thoigianCongBo\":\"\",\"typeName\":\"GetListSoLieu\",\"language\":\"TIENG_VIET\"}";
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "text/plain");

                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var responseStr =await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GOV_HaiQuanResponse>(responseStr);
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.TongCucHaiQuan|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> TongCucHaiQuan(string url)
        {
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var stream = await responseMessage.Content.ReadAsStreamAsync();
                return stream;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.TongCucHaiQuan|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> TongCucThongKeTest(DateTime dt)
        {
            try
            {
                //LV1
                var url = "https://www.gso.gov.vn/bao-cao-tinh-hinh-kinh-te-xa-hoi-hang-thang/?paged=2";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var linkedPages = doc.DocumentNode.Descendants("a")
                                                  .Select(a => a.GetAttributeValue("href", null))
                                                  .Where(u => !string.IsNullOrWhiteSpace(u))
                                                  .Where(x => x.Contains(dt.Year.ToString()) && x.Contains(dt.Month.To2Digit().ToString()));

                var link = linkedPages.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(link))
                    return null;
                //LV2
                var clientDetail = new HttpClient { BaseAddress = new Uri(link) };
                responseMessage = await clientDetail.GetAsync("", HttpCompletionOption.ResponseContentRead);
                html = await responseMessage.Content.ReadAsStringAsync();
                doc.LoadHtml(html);
                linkedPages = doc.DocumentNode.Descendants("a")
                                                  .Select(a => a.GetAttributeValue("href", null))
                                                  .Where(u => !string.IsNullOrWhiteSpace(u))
                                                  .Where(x => x.Contains(dt.Year.ToString()) && x.Contains(dt.Month.To2Digit().ToString()) && x.Contains(".xlsx"));
                link = linkedPages.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(link))
                    return null;
                //LV3
                var clientDownload = new HttpClient { BaseAddress = new Uri(link) };
                responseMessage = await clientDownload.GetAsync("", HttpCompletionOption.ResponseContentRead);
                return await responseMessage.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.TongCucThongKe|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private class SSI_DataTradingResponse
        {
            public SSI_DataTradingDetailResponse data { get; set; }
        }

        private class SSI_DataTradingDetailResponse
        {
            public IEnumerable<decimal> t { get; set; }
            public IEnumerable<decimal> c { get; set; }
            public IEnumerable<decimal> o { get; set; }
            public IEnumerable<decimal> h { get; set; }
            public IEnumerable<decimal> l { get; set; }
            public IEnumerable<decimal> v { get; set; }
        }

        public class Money24h_ForeignAPIResponse
        {
            public Money24h_ForeignAPI_DataResponse data { get; set; }
            public int status { get; set; }
        }

        public class Money24h_ForeignAPI_DataResponse
        {
            public List<Money24h_ForeignAPI_DataDetailResponse> data { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        public class Money24h_ForeignAPI_DataDetailResponse
        {
            public string symbol { get; set; }
            public decimal sell_qtty { get; set; }
            public decimal sell_val { get; set; }
            public decimal buy_qtty { get; set; }
            public decimal buy_val { get; set; }
            public decimal net_val { get; set; }
        }
    }
}

