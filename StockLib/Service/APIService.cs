using HtmlAgilityPack;
using HttpClientToCurl;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using StockLib.Model;
using StockLib.Service.Settings;
using StockLib.Utils;
using System.Text;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

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
        Task<VietStock_Forex> VietStock_GetForex(string code);

        Task<Stream> GetChartImage(string body);

        Task<List<Money24h_PTKTResponse>> Money24h_GetMaTheoChiBao_MA20();
        Task<List<Money24h_PTKTResponse>> Money24h_GetMaTheoChiBao_52W();
        Task<Money24h_NhomNganhResponse> Money24h_GetNhomNganh(EMoney24hTimeType type);
        Task<List<Money24h_ForeignResponse>> Money24h_GetForeign(EExchange mode, EMoney24hTimeType type);
        Task<List<Money24h_KeHoach_Data>> Money24h_GetKeHoach(string code);


        Task<List<Quote>> SSI_GetDataStock(string code);
        Task<List<Quote>> SSI_GetDataStock_Alltime(string code);
        Task<List<SSI_PEDetail>> SSI_GetFinance(string code);
        Task<SSI_Share> SSI_GetShare(string code);
        //
        Task<List<DSC_Data>> DSC_GetPost();
        Task<List<VNDirect_Data>> VNDirect_GetPost();
        Task<List<MigrateAsset_Data>> MigrateAsset_GetPost();
        Task<List<AGR_Data>> Agribank_GetPost();
        Task<List<BCPT_Crawl_Data>> SSI_GetPost();
        Task<List<BCPT_Crawl_Data>> BSC_GetPost();
        Task<List<VCBS_Data>> VCBS_GetPost();
        Task<List<BCPT_Crawl_Data>> MBS_GetPost();
        Task<List<BCPT_Crawl_Data>> PSI_GetPost();
        Task<List<BCPT_Crawl_Data>> CafeF_GetPost();

        Task<List<MacroVar_Data>> MacroVar_GetData(string id);

        Task<Stream> TuDoanhHNX(EHnxExchange mode, DateTime dt);
        Task<Stream> TuDoanhHSX(DateTime dt);
        Task<Stream> TongCucThongKe(DateTime dt);
        Task<GOV_HaiQuanResponse> TongCucHaiQuan();
        Task<Stream> TongCucHaiQuan(string url);
        Task<Stream> TongCucThongKeTest(DateTime dt);
        Task<string> TongCucThongKe();
        Task<List<string>> DSTongCucThongKe();
        Task<Stream> StreamTongCucThongKe(string url);

        Task<double> Tradingeconimic_GetForex(string code);
        Task<List<TradingEconomics_Data>> Tradingeconimic_Commodities();
        Task<(float, float)> Drewry_WCI();
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
                client.Timeout = TimeSpan.FromSeconds(5);
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

        public async Task<VietStock_Forex> VietStock_GetForex(string code)
        {
            try
            {
                var dt = DateTimeOffset.Now;
                var url = $"https://api.vietstock.vn/tvnew/history?symbol={code}&resolution=1D&from={dt.AddYears(-1).AddMonths(-3).ToUnixTimeSeconds()}&to={dt.ToUnixTimeSeconds()}&countback=500";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("Referer", "https://stockchart.vietstock.vn/");
                requestMessage.Method = HttpMethod.Get;
                requestMessage.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                var responseMessage = await client.SendAsync(requestMessage);
                if(responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return null;
                }
                var resultStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<VietStock_Forex>(resultStr);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VietStock_GetDanhSachBCTC|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        #endregion

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
                client.Timeout = TimeSpan.FromSeconds(5);
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

        public async Task<List<Money24h_ForeignResponse>> Money24h_GetForeign(EExchange mode, EMoney24hTimeType type)
        {
            var lOutput = new List<Money24h_ForeignResponse>();
            try
            {
                var urlBase = "https://api-finance-t19.24hmoney.vn/v2/web/indices/foreign-trading-all-stock-by-time?code={0}&type={1}";
                var url = string.Format(urlBase, mode.GetDisplayName(), type.GetDisplayName());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
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
                client.Timeout = TimeSpan.FromSeconds(5);
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
            Thread.Sleep(200);
            return lOutput;
        }

        public async Task<List<Quote>> SSI_GetDataStock_Alltime(string code)
        {
            var lOutput = new List<Quote>();
            var urlBase = "https://iboard-api.ssi.com.vn/statistics/charts/history?symbol={0}&resolution={1}&from={2}&to={3}";
            try
            {
                var dt = DateTimeOffset.Now;
                var div = 0;
                while (true)
                {
                    var dtFirst = dt.AddYears(-div);
                    var dtLast = dt.AddYears(-(div + 2));
                    if (dtLast.Year < 2000)
                        break;

                    div += 2;

                    var url = string.Format(urlBase, code, "1D", dtLast.ToUnixTimeSeconds(), dtFirst.ToUnixTimeSeconds());
                    var client = _client.CreateClient();
                    client.BaseAddress = new Uri(url);
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                    var resultArray = await responseMessage.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<SSI_DataTradingResponse>(resultArray);
                    if (responseModel.data.t.Any())
                    {
                        var count = responseModel.data.t.Count();
                        if (count <= 100)
                            break;

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
                    Thread.Sleep(1000);
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
                client.Timeout = TimeSpan.FromSeconds(5);
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

        public async Task<SSI_Share> SSI_GetShare(string code)
        {
            var url = $"https://iboard-api.ssi.com.vn/statistics/company/company-statistics?symbol={code}";
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<SSI_ShareResponse>(responseMessageStr);
                return responseModel.data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.SSI_GetShare|EXCEPTION| {ex.Message}");
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
                client.Timeout = TimeSpan.FromSeconds(5);
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
                client.Timeout = TimeSpan.FromSeconds(5);
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
                client.Timeout = TimeSpan.FromSeconds(5);
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
                clientDetail.Timeout = TimeSpan.FromSeconds(5);
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

        public async Task<string> TongCucThongKe()
        {
            try
            {
                var lLink = new List<string>();
                for (int i = 1; i <= 1; i++)
                {
                    var url = $"https://www.gso.gov.vn/bao-cao-tinh-hinh-kinh-te-xa-hoi-hang-thang/?paged={i}";
                    var client = _client.CreateClient();
                    client.BaseAddress = new Uri(url);
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                    var html = await responseMessage.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var linkedPages = doc.DocumentNode.Descendants("a")
                                                      .Select(a => a.GetAttributeValue("href", null))
                                                      .Where(u => !string.IsNullOrWhiteSpace(u))
                                                      .Where(x => (x.Contains("2024") || x.Contains("2023"))
                                                                && (x.Contains("01") || x.Contains("02") || x.Contains("03") || x.Contains("04") || x.Contains("05") || x.Contains("06")
                                                                    || x.Contains("07") || x.Contains("08") || x.Contains("09") || x.Contains("10") || x.Contains("11") || x.Contains("12")));

                    lLink.AddRange(linkedPages);
                }
                //each link
                foreach (var item in lLink)
                {
                    var clientDetail = new HttpClient { BaseAddress = new Uri(item) };
                    clientDetail.Timeout = TimeSpan.FromSeconds(5);
                    var responseMessage = await clientDetail.GetAsync("", HttpCompletionOption.ResponseContentRead);
                    var html = await responseMessage.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var linkedPages = doc.DocumentNode.Descendants("a")
                                                      .Select(a => a.GetAttributeValue("href", null))
                                                      .Where(u => !string.IsNullOrWhiteSpace(u))
                                                      .Where(x => x.Contains(".xlsx"));
                    return linkedPages.FirstOrDefault();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.TongCucThongKe|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<string>> DSTongCucThongKe()
        {
            try
            {
                var lLink = new List<string>();
                for (int i = 1; i <= 3; i++)
                {
                    var url = $"https://www.gso.gov.vn/bao-cao-tinh-hinh-kinh-te-xa-hoi-hang-thang/?paged={i}";
                    var client = _client.CreateClient();
                    client.BaseAddress = new Uri(url);
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                    var html = await responseMessage.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var linkedPages = doc.DocumentNode.Descendants("a")
                                                      .Select(a => a.GetAttributeValue("href", null))
                                                      .Where(u => !string.IsNullOrWhiteSpace(u))
                                                      .Where(x => (x.Contains("2024") || x.Contains("2023")) 
                                                                && (x.Contains("01") || x.Contains("02") || x.Contains("03") || x.Contains("04") || x.Contains("05") || x.Contains("06")
                                                                    || x.Contains("07") || x.Contains("08") || x.Contains("09") || x.Contains("10") || x.Contains("11") || x.Contains("12")));

                    lLink.AddRange(linkedPages);
                }
                //each link
                var lExcel = new List<string>();
                foreach (var item in lLink)
                {
                    var clientDetail = new HttpClient { BaseAddress = new Uri(item) };
                    clientDetail.Timeout = TimeSpan.FromSeconds(5);
                    var responseMessage = await clientDetail.GetAsync("", HttpCompletionOption.ResponseContentRead);
                    var html = await responseMessage.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var linkedPages = doc.DocumentNode.Descendants("a")
                                                      .Select(a => a.GetAttributeValue("href", null))
                                                      .Where(u => !string.IsNullOrWhiteSpace(u))
                                                      .Where(x => x.Contains(".xlsx"));
                    lExcel.AddRange(linkedPages);
                }

                return lExcel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.DSTongCucThongKe|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<Stream> StreamTongCucThongKe(string url)
        {
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
                return await responseMessage.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.StreamTongCucThongKe|EXCEPTION| {ex.Message}");
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
                client.Timeout = TimeSpan.FromSeconds(5);
                var requestMessage = new HttpRequestMessage();
                var body = "{\"skip\":0,\"take\":10,\"ky\":\"\",\"textSearch\":\"\",\"the_loai\":\"0\",\"thoigianCongBo\":\"\",\"typeName\":\"GetListSoLieu\",\"language\":\"TIENG_VIET\"}";
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
                client.Timeout = TimeSpan.FromSeconds(5);
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
                client.Timeout = TimeSpan.FromSeconds(5);
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

        public async Task<List<DSC_Data>> DSC_GetPost()
        {
            var url = $"https://www.dsc.com.vn/_next/data/83x7wmD1bec8LOqcLMWqa/bao-cao-phan-tich/tat-ca-bao-cao.json?slug=tat-ca-bao-cao";
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<DSC_Main>(responseMessageStr);
                return responseModel?.pageProps?.dataCategory?.dataList?.data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.DSC_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<VNDirect_Data>> VNDirect_GetPost()
        {
            var url = $"https://api-finfo.vndirect.com.vn/v4/news?q=newsType:company_report~locale:VN~newsSource:VNDIRECT&sort=newsDate:desc~newsTime:desc&size=20";
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<VNDirect_Main>(responseMessageStr);
                return responseModel?.data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VNDirect_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<MigrateAsset_Data>> MigrateAsset_GetPost()
        {
            var url = $"https://masvn.com/api/categories/fe/56/article?paging=1&sort=published_at&direction=desc&active=1&page=1&limit=10";
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<MigrateAsset_Main>(responseMessageStr);
                return responseModel?.data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.MigrateAsset_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<AGR_Data>> Agribank_GetPost()
        {
            var url = $"https://agriseco.com.vn/api/Data/Report/SearchReports?categoryID=1&sourceID=5&sectorID=null&symbol=&keywords=&startDate=2022/10/6&endDate=2024/10/7&startIndex=0&count=10";
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<List<AGR_Data>>(responseMessageStr);
                return responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Agribank_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<VCBS_Data>> VCBS_GetPost()
        {
            var url = $"https://www.vcbs.com.vn/api/v1/ttpt-reports?limit=15&page=1&keyword=&locale=vi";
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<VCBS_Main>(responseMessageStr);
                return responseModel.data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.VCBS_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<BCPT_Crawl_Data>> SSI_GetPost()
        {
            try
            {
                var lResult = new List<BCPT_Crawl_Data>();
                var link = string.Empty;
                var url = $"https://www.ssi.com.vn/khach-hang-ca-nhan/bao-cao-cong-ty";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                //var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                for (int i = 0; i < 15; i++)
                {
                    var nodeCode = doc.DocumentNode.SelectSingleNode($"/html/body/main/section[2]/div/div[2]/div[2]/div/div[2]/div[{i+1}]/div[1]/a");
                    var nodeTime = doc.DocumentNode.SelectSingleNode($"/html/body/main/section[2]/div/div[2]/div[2]/div/div[2]/div[{i + 1}]/div[2]/p/span");
                    var title = nodeCode?.InnerText.Replace("\n", "").Trim();
                    var timeStr = nodeTime?.InnerText.Trim();
                    if (string.IsNullOrWhiteSpace(timeStr))
                        continue;

                    var strSplit = timeStr.Split('/');
                    if(strSplit.Length == 3 && !string.IsNullOrWhiteSpace(title))
                    {
                        var year = int.Parse(strSplit[2].Trim());
                        var month = int.Parse(strSplit[1].Trim());
                        var day = int.Parse(strSplit[0].Trim());
                        lResult.Add(new BCPT_Crawl_Data
                        {
                            id = $"{strSplit[2].Trim()}{strSplit[1].Trim()}{strSplit[0].Trim()}{title.Substring(0, 3)}",
                            title = title,
                            date = new DateTime(year, month, day)
                        });
                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.SSI_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<BCPT_Crawl_Data>> BSC_GetPost()
        {
            try
            {
                var lResult = new List<BCPT_Crawl_Data>();
                var link = string.Empty;
                var url = $"https://www.bsc.com.vn/bao-cao-phan-tich/danh-muc-bao-cao/1";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                //var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                for (int i = 0; i < 10; i++)
                {
                    var nodeCode = doc.DocumentNode.SelectSingleNode($"/html/body/div[3]/div[3]/div[4]/div[4]/div[2]/div/table/tbody/tr[{i+1}]/td[2]/a");
                    var nodeTime = doc.DocumentNode.SelectSingleNode($"/html/body/div[3]/div[3]/div[4]/div[4]/div[2]/div/table/tbody/tr[{i + 1}]/td[1]");
                    var title = nodeCode?.InnerText.Replace("\n", "").Trim();
                    var timeStr = nodeTime?.InnerText.Trim();
                    if (string.IsNullOrWhiteSpace(timeStr))
                        continue;

                    var strSplit = timeStr.Split('/');
                    if (strSplit.Length == 3 && !string.IsNullOrWhiteSpace(title))
                    {
                        var year = int.Parse(strSplit[2].Trim());
                        var month = int.Parse(strSplit[1].Trim());
                        var day = int.Parse(strSplit[0].Trim());
                        lResult.Add(new BCPT_Crawl_Data
                        {
                            id = $"{strSplit[2].Trim()}{strSplit[1].Trim()}{strSplit[0].Trim()}{title.Substring(0, 3)}",
                            title = title,
                            date = new DateTime(year, month, day)
                        });
                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.BSC_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<BCPT_Crawl_Data>> MBS_GetPost()
        {
            try
            {
                var lResult = new List<BCPT_Crawl_Data>();
                var link = string.Empty;
                var url = $"https://mbs.com.vn/trung-tam-nghien-cuu/bao-cao-phan-tich/nghien-cuu-co-phieu/";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                for (int i = 0; i < 10; i++)
                {
                    var nodeCode = doc.DocumentNode.SelectSingleNode($"//*[@id=\"content\"]/div/div/div[2]/main/section[2]/div/div[1]/div[{i + 1}]/div/a");
                    var nodeTime = doc.DocumentNode.SelectSingleNode($"//*[@id=\"content\"]/div/div/div[2]/main/section[2]/div/div[1]/div[{i + 1}]/div/div[1]");
                    var title = nodeCode?.InnerText.Replace("\n", "").Trim();
                    var timeStr = nodeTime?.InnerText.Trim();
                    if (string.IsNullOrWhiteSpace(timeStr))
                        continue;

                    var strSplit = timeStr.Split('/');
                    if (strSplit.Length == 3 && !string.IsNullOrWhiteSpace(title))
                    {
                        var year = int.Parse(strSplit[2].Trim());
                        var month = int.Parse(strSplit[1].Trim());
                        var day = int.Parse(strSplit[0].Trim());
                        lResult.Add(new BCPT_Crawl_Data
                        {
                            id = $"{strSplit[2].Trim()}{strSplit[1].Trim()}{strSplit[0].Trim()}{title.Substring(0, 3)}",
                            title = title,
                            date = new DateTime(year, month, day)
                        });
                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.MBS_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<BCPT_Crawl_Data>> PSI_GetPost()
        {
            try
            {
                var lResult = new List<BCPT_Crawl_Data>();
                var link = string.Empty;
                var url = $"https://www.psi.vn/vi/trung-tam-phan-tich/bao-cao-phan-tich-doanh-nghiep";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                for (int i = 0; i < 10; i++)
                {
                    var nodeCode = doc.DocumentNode.SelectSingleNode($"/html/body/div[3]/div[3]/div[{i + 1}]/div[2]/div[1]/div[1]");
                    var nodeTime = doc.DocumentNode.SelectSingleNode($"/html/body/div[3]/div[3]/div[{i + 1}]/div[1]/div/div[1]");
                    var title = nodeCode?.InnerText.Replace("\n", "").Trim();
                    var timeStr = nodeTime?.InnerText.Trim().Replace("\n","/");
                    if (string.IsNullOrWhiteSpace(timeStr))
                        continue;

                    var strSplit = timeStr.Split('/');
                    if (strSplit.Length == 3 && !string.IsNullOrWhiteSpace(title))
                    {
                        var year = int.Parse(strSplit[2].Trim());
                        var month = int.Parse(strSplit[1].Trim());
                        var day = int.Parse(strSplit[0].Trim());
                        lResult.Add(new BCPT_Crawl_Data
                        {
                            id = $"{strSplit[2].Trim()}{strSplit[1].Trim()}{strSplit[0].Trim()}{title.Substring(0, 3)}",
                            title = title,
                            date = new DateTime(year, month, day)
                        });
                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.PSI_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<BCPT_Crawl_Data>> CafeF_GetPost()
        {
            try
            {
                var lResult = new List<BCPT_Crawl_Data>();
                var link = string.Empty;
                var url = $"https://s.cafef.vn/phan-tich-bao-cao.chn";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                for (int i = 0; i < 10; i++)
                {
                    var nodeCode = doc.DocumentNode.SelectSingleNode($"//*[@id=\"ContentPlaceHolder1_AnalyzeReportList1_rptData_itemTR_{i}\"]/td[2]/a");
                    var nodeTime = doc.DocumentNode.SelectSingleNode($"//*[@id=\"ContentPlaceHolder1_AnalyzeReportList1_rptData_itemTR_{i}\"]/td[1]");
                    var title = nodeCode?.InnerText.Replace("\n", "").Trim();
                    var timeStr = nodeTime?.InnerText.Trim();
                    if (string.IsNullOrWhiteSpace(timeStr))
                        continue;

                    var strSplit = timeStr.Split('/');
                    if (strSplit.Length == 3 && !string.IsNullOrWhiteSpace(title))
                    {
                        var year = int.Parse(strSplit[2].Trim());
                        var month = int.Parse(strSplit[1].Trim());
                        var day = int.Parse(strSplit[0].Trim());
                        lResult.Add(new BCPT_Crawl_Data
                        {
                            id = $"{strSplit[2].Trim()}{strSplit[1].Trim()}{strSplit[0].Trim()}{title.Substring(0, 3)}",
                            title = title,
                            date = new DateTime(year, month, day)
                        });
                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.CafeF_GetPost|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<List<MacroVar_Data>> MacroVar_GetData(string id)
        {
            var url = $"https://macrovar.com/wp-json/mesmerize-api/v1/market-data-wid";
            var body = $"wid={id}";
            try
            {
                var client = _client.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                var responseMessage = await client.SendAsync(requestMessage);

                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;

                var responseMessageStr = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<MacroVar_Main>(responseMessageStr);
                return responseModel.data;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.MacroVar_BDTI|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<double> Tradingeconimic_GetForex(string code)
        {
            try
            {
                //LV1
                var link = string.Empty;
                var url = $"https://tradingeconomics.com/commodity/{code}";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(5);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                //var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"aspnetForm\"]/div[5]/div/div[1]/div[4]/div/div/table/tr") as IEnumerable<HtmlNode>;
                var lVal = new List<string>();
                var istrue = false;
                var iscomplete = false;
                foreach (var item in nodes)
                {
                    foreach (HtmlNode node in item.ChildNodes)
                    {
                        if (string.IsNullOrWhiteSpace(node.InnerText))
                            continue;

                        if (!istrue && node.InnerText.RemoveSpace().Replace("-", "").Contains(code.RemoveSpace().Replace("-", ""), StringComparison.OrdinalIgnoreCase))
                        {
                            istrue = true;
                            continue;
                        }

                        if (!istrue)
                            continue;

                        if (!node.InnerText.Contains("%"))
                            continue;
                        lVal.Add(node.InnerText.Replace("%", ""));
                        iscomplete = true;
                    }
                    if (iscomplete)
                        break;
                }

                if (lVal.Any())
                {
                    var isDouble = double.TryParse(lVal.Last(), out var val);
                    if (isDouble)
                        return val;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Tradingeconimic_GetForex|EXCEPTION| {ex.Message}");
            }
            return 0;
        }

        public async Task<List<TradingEconomics_Data>> Tradingeconimic_Commodities()
        {
            try
            {
                var lCode = new List<string>
                {
                    "Crude Oil",//Dầu thô
                    "Natural gas",//Khí thiên nhiên
                    "Coal",//Than
                    "Gold",//Vàng
                    "Steel",//Thép
                    "HRC Steel",//Thép HRC
                    "Rubber", //Cao su
                    "Coffee", //Cà phê
                    "Rice", //Gạo
                    "Sugar", //Đường
                    "Urea", //U rê
                };

                var lResult = new List<TradingEconomics_Data>();

                //LV1
                var link = string.Empty;
                var url = $"https://tradingeconomics.com/commodities";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(15);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                //var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var tableNodes = doc.DocumentNode.SelectNodes("//table");
                foreach (var item in tableNodes)
                {
                    var tbody = item.ChildNodes["tbody"];
                    foreach (var row in tbody.ChildNodes.Where(r => r.Name == "tr"))
                    {
                        var model = new TradingEconomics_Data();
                        var columnsArray = row.ChildNodes.Where(c => c.Name == "td").ToArray();
                        for (int i = 0; i < columnsArray.Length; i++)
                        {
                            if (i == 0)
                            {
                                model.Code = columnsArray[i].InnerText.Trim().Split("\r")[0].Trim();
                            }
                            else if (i == 4)
                            {
                                var isFloat = float.TryParse(columnsArray[i].InnerText.Replace("%", "").Trim(), out var val);
                                if (isFloat)
                                {
                                    model.Weekly = val;
                                }
                            }
                            else if (i == 5)
                            {
                                var isFloat = float.TryParse(columnsArray[i].InnerText.Replace("%", "").Trim(), out var val);
                                if (isFloat)
                                {
                                    model.Monthly = val;
                                }
                            }
                            else if (i == 6)
                            {
                                var isFloat = float.TryParse(columnsArray[i].InnerText.Replace("%", "").Trim(), out var val);
                                if (isFloat)
                                {
                                    model.YTD = val;
                                }
                            }
                            else if (i == 7)
                            {
                                var isFloat = float.TryParse(columnsArray[i].InnerText.Replace("%", "").Trim(), out var val);
                                if (isFloat)
                                {
                                    model.YoY = val;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(model.Code)
                            && lCode.Contains(model.Code))
                        {
                            lResult.Add(model);
                        }
                    }
                }

                return lResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Tradingeconimic_Commodities|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        public async Task<(float, float)> Drewry_WCI()
        {
            try
            {
                var link = string.Empty;
                var url = $"https://www.drewry.co.uk/supply-chain-advisors/supply-chain-expertise/world-container-index-assessed-by-drewry";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(15);

                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Get;
                var responseMessage = await client.SendAsync(requestMessage);

                //var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var urls = doc.DocumentNode.Descendants("img")
                                .Select(e => e.GetAttributeValue("src", null))
                                .Where(s => !String.IsNullOrEmpty(s));
                var truePath = urls.FirstOrDefault(x => x.Contains("WCI-Table"));
                if (truePath != null)
                {
                    //Call Next
                    var clientDetail = new HttpClient { BaseAddress = new Uri(truePath) };
                    responseMessage = await clientDetail.GetAsync("", HttpCompletionOption.ResponseContentRead);
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    doc.LoadHtml(content);
                    var gs = doc.DocumentNode.Descendants("tspan");
                    var count = 0;
                    var isFirst = true;
                    var flag = false;
                    var sBuilder = new StringBuilder();

                    foreach (var item in gs)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                            continue;
                        }

                        if (flag)
                        {
                            sBuilder.AppendLine(item.InnerText);
                            if(item.InnerText.Contains("%"))
                            {
                                count++;
                            }

                            if (count >= 2)
                                break;
                        }

                        if (item.InnerText.Contains("$"))
                        {
                            count++;
                        }

                        if(count >= 3)
                        {
                            flag = true;
                            count = 0;
                            continue;
                        }
                    }

                    var strSplit = sBuilder.ToString().Split("%");
                    if(strSplit.Length >= 2)
                    {
                        var isFloat1 = float.TryParse(strSplit[0].Replace("\r\n", ""), out var val1);
                        var isFloat2 = float.TryParse(strSplit[1].Replace("\r\n", ""), out var val2);
                        return (val1, val2);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.Drewry_WCI|EXCEPTION| {ex.Message}");
            }
            return (0, 0);
        }

        public async Task<Stream> GetChartImage(string body)
        {
            try
            {
                var url = ServiceSetting._chartLocal;
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromSeconds(10);
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

