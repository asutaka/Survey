using HtmlAgilityPack;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using SLib.Model;
using SLib.Model.APIModel;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SLib.Service
{
    public interface IAPIService
    {
        Task<Stream> GetTuDoanhHNX(EHnxExchange mode);
        Task<Stream> GetTuDoanhHSX();
        Task<List<Quote>> GetDataStock(string code);
        Task<List<TuDoanh>> GetTuDoanh24H();
        Task<List<Foreign>> GetGDNN24H(E24HGDNNMode mode, E24hGDNNType type);
        Task<string> GetExternalIpAddress();
        Task<NhomNganhAPIModel> GetDulieuNhomNganh(E24hGDNNType type);
        Task<MaTheoNhomNganhAPIModel> GetMaTheoNhomNganh(string nhom);
        Task<List<DuLieuTheoChiBaoAPIDataDetailModel>> GetMaTheoChiBao();
        Task<KeHoachThucHienAPIModel> GetKeHoachThucHien(string ma);
        Task<List<LoiNhuanAPIDetail>> ThongKeLoiNhuan(string ma);
        Task<List<ForeignDataModel>> GetForeign(string code, int page, int pageSize, string fromDate, string toDate);
        Task<IEnumerable<BCTCAPIModel>> GetDanhSachBCTC(string code);
        Task<Stream> GetChartImage(string body);
        Task<List<Financial>> GetDoanhThuLoiNhuan(string code);
    }
    public class APIService : IAPIService
    {
        private readonly IHttpClientFactory _client;
        public APIService(IHttpClientFactory httpClientFactory) 
        {
            _client = httpClientFactory; 
        }
        public async Task<Stream> GetTuDoanhHNX(EHnxExchange mode)
        {
            try
            {
                var dt = DateTime.Now;
                var strDate = $"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}";
                var url = string.Format(ServiceSetting._tudoanhHNX, strDate, mode.ToString());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                return await responseMessage.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<Stream> GetTuDoanhHSX()
        {
            try
            {
                //LV1
                var link = string.Empty;
                var dt = DateTime.Now;
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
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<Quote>> GetDataStock(string code)
        {
            var lOutput = new List<Quote>();
            try
            {
                var url = string.Format(ServiceSetting._stockData, code, "1D", DateTimeOffset.Now.AddYears(-2).ToUnixTimeSeconds(), DateTimeOffset.Now.ToUnixTimeSeconds());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<StockDataSurroundModel>(resultArray);
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
                Console.WriteLine(ex.Message);
            }
            return lOutput;
        }

        public async Task<List<TuDoanh>> GetTuDoanh24H()
        {
            var lOutput = new List<TuDoanh>();
            try
            {
                var url = ServiceSetting._giaodichTuDoanh_24hMoney;
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<TudoanhAPI>(resultArray);
                if (responseModel.status == 200 
                    && responseModel.data.data.Any())
                {
                    var date = responseModel.data.from_date.ToDateTime("dd/MM/yyyy");
                    return responseModel.data.data.Where(x => x.symbol.Length == 3).OrderByDescending(x => x.prop_net).Select((x, index) => new TuDoanh
                    {
                        no = index + 1,
                        d = new DateTimeOffset(date, TimeSpan.FromHours(0)).ToUnixTimeSeconds(),
                        s = x.symbol,
                        net_deal = x.prop_net_deal,
                        net_pt = x.prop_net_pt,
                        net = x.prop_net,
                        t = DateTimeOffset.Now.ToUnixTimeSeconds()
                    }).ToList(); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lOutput;
        }

        public async Task<List<Foreign>> GetGDNN24H(E24HGDNNMode mode, E24hGDNNType type)
        {
            var lOutput = new List<Foreign>();
            try
            {
                var url = string.Format(ServiceSetting._giaodichNN_24hMoney, mode.GetDisplayName(), type.GetDisplayName());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<GDNNAPIModel>(resultArray);
                if (responseModel.status == 200
                    && responseModel.data.data.Any())
                {
                    var date = responseModel.data.from_date.ToDateTime("dd/MM/yyyy");
                    if(date.Day == DateTime.Now.Day)
                    {
                        return responseModel.data.data.Where(x => x.symbol.Length == 3).OrderByDescending(x => x.net_val).Select((x, index) => new Foreign
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
                Console.WriteLine(ex.Message);
            }
            return lOutput;
        }

        public async Task<string> GetExternalIpAddress()
        {
            try
            {
                var url = "http://icanhazip.com";
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                return await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public async Task<NhomNganhAPIModel> GetDulieuNhomNganh(E24hGDNNType type)
        {
            try
            {
                var url = string.Format(ServiceSetting._nhomNganh_24hMoney, type.GetDisplayName());
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var result = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<NhomNganhAPIModel>(result);
                if (responseModel.status == 200)
                {
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<MaTheoNhomNganhAPIModel> GetMaTheoNhomNganh(string nhom)
        {
            try
            {
                var url = string.Format(ServiceSetting._maTheoNganh_24hMoney, nhom);
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var result = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<MaTheoNhomNganhAPIModel>(result);
                if (responseModel.status == 200)
                {
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<DuLieuTheoChiBaoAPIDataDetailModel>> GetMaTheoChiBao()
        {
            var body = "{\"floor_codes\":[\"10\",\"11\",\"02\",\"03\"],\"group_ids\":[\"0001\",\"1000\",\"2000\",\"3000\",\"4000\",\"5000\",\"6000\",\"7000\",\"8000\",\"8301\",\"9000\"],\"signals\":[{\"ma20\":\"up\"}]}";
            var lOutput = new List<DuLieuTheoChiBaoAPIDataDetailModel>();
            try
            {
                var url = ServiceSetting._maTheoChiBao_24hMoney;
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json"); 
               
                var responseMessage = await client.SendAsync(requestMessage);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<DuLieuTheoChiBaoAPIModel>(resultArray);
                if (responseModel.status == 200
                    && responseModel.data.total_symbol > 0
                    && responseModel.data.data.Any())
                {
                    return responseModel.data.data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lOutput;
        }

        public async Task<KeHoachThucHienAPIModel> GetKeHoachThucHien(string ma)
        {
            try
            {
                var url = string.Format(ServiceSetting._keHoachThucHien_24hMoney, ma);
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var result = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<KeHoachThucHienAPIModel>(result);
                if (responseModel.status == 200)
                {
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<LoiNhuanAPIDetail>> ThongKeLoiNhuan(string ma)
        {
            try
            {
                var url = string.Format(ServiceSetting._loinhuan_24hMoney, ma);
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var result = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<LoiNhuanAPIModel>(result);
                if (responseModel.status == 200)
                {
                    var lOutput = new List<LoiNhuanAPIDetail>();
                    foreach (var item in responseModel.data.xAxis)
                    {
                        var first = responseModel.data.points.FirstOrDefault(x => x.x == item.value);
                        if (first is null)
                            continue;

                        lOutput.Add(new LoiNhuanAPIDetail
                        {
                            Name = item.name,
                            Profit = first.y1,
                            Rate_qoq = first.y3
                        });
                    }
                    return lOutput;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<ForeignDataModel>> GetForeign(string code, int page, int pageSize, string fromDate, string toDate)
        {
            try
            {
                var url = string.Format(ServiceSetting._foreignBuySell_ssi, code, page, pageSize, UrlEncoder.Default.Encode(fromDate), UrlEncoder.Default.Encode(toDate));
                var client = new HttpClient { BaseAddress = new Uri(url) };
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ForeignModel>(resultArray);
                return res.data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"APIService.GetForeign|EXCEPTION|INPUT: {code}| {ex.Message}");
            }
            return null;
        }

        public async Task<IEnumerable<BCTCAPIModel>> GetDanhSachBCTC(string code)
        {
            var body = $"code={code}&page=1&type=1&__RequestVerificationToken=7E2f40aB2GtqaSRw_1PuR4Fa1x1YHJNqUonxZP9-xlcBpLR9Gh_8ksrtVeyg6dm51Tj-1KY22gRn-ZeDPt_1Sz1oSqFLJKaikrbC5x-Z7L7HEVisBf4QwOUhPhrtbqll0";
            try
            {
                var url = ServiceSetting._bctc_vietstock;
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                //requestMessage.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                requestMessage.Headers.Add("Cookie", "ASP.NET_SessionId=kez23nkspuoomciouahd1xqp; __RequestVerificationToken=5t0qgD3M2IWZKLXukNsWaFE2ZCWl_cKVOn2SDHUDDw6NIEfBM1FC1HWEnrE9BzsrKeZrRWbGyYItV21WS4E6t-CTsKZqRvQIv6Ma5qAegwU1; language=vi-VN; vts_usr_lg=8CA082FB9A83611E2EE754B3D0817C20650DC01BCB8E76FCAC643E2DE3B937D04A76BC9A8E76AC797AA7E895EF0AE9051A1FCB4BD46B023E8480A86FFC74A037B20716A602AF53D46BA8F61510D3143F5D712AE2DC4C6A15D16986FE3C2BC95C852ED024741320D07D992CAFF99D22E8C5DB03AB74F593A2517D2B63745AFA70; _ga=GA1.1.1323687995.1720524498; _ga_EXMM0DKVEX=GS1.1.1720524497.1.0.1720524497.60.0.0; Theme=Light; finance_viewedstock=VHM,SHS,SZC,; ASP.NET_SessionId=uybogztjonmgl4zs4blcqcn2; __RequestVerificationToken=i57_ExLOX1hZcmxGXejYGH720nWhDh_sCb42qkKM98h4gGstmLBzGk5cNOEPN_Ir470i5kklT4ZyjOickCYIa6ZbvBbJu0a-VVwD_ffRwn01; language=vi-VN");
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                var responseMessage = await client.SendAsync(requestMessage);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<IEnumerable<BCTCAPIModel>>(resultArray);
                return responseModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<Stream> GetChartImage(string body)
        {
            try
            {
                var url = ServiceSetting._chart;
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
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<Financial>> GetDoanhThuLoiNhuan(string code)
        {
            var lOutput = new List<Financial>();
            try
            {
                var url = string.Format(ServiceSetting._financeInfo_ssi, code);
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<FinancialAPIModel>(resultArray);
                if (responseModel is null)
                    return null;
                return responseModel.data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lOutput;
        }
    }
}
