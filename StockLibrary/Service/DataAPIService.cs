using HtmlAgilityPack;
using iTextSharp.text.pdf.qrcode;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using StockLibrary.Model.APIModel;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace StockLibrary.Service
{
    public interface IDataAPIService
    {
        Task<List<string>> GetStock(EStockExchange param);
        Task<CompanyProfileDataModel> GetCompanyInfo(string code);
        Task<List<ShareHolderDataModel>> GetShareHolderCompany(string code);
        Task<ForeignModel> GetForeign(string code, int page, int pageSize, string fromDate, string toDate);
        Task<List<Quote>> GetDataStock(string code);
        Task<Stream> GetTuDoanhHNX(EHnxExchange mode);
        Task<Stream> GetTuDoanhHSX();
    }
    public class DataAPIService : IDataAPIService
    {
        public async Task<List<string>> GetStock(EStockExchange param)
        {
            try
            {
                var url = string.Format(ServiceSetting._stockExchange, param.GetDisplayName());
                var client = new HttpClient { BaseAddress = new Uri(url) };
                var responseMessage = await client.GetAsync("");
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var lData = JsonConvert.DeserializeObject<List<string>>(resultArray);
                return lData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataAPIService.GetStock|EXCEPTION|INPUT: {JsonConvert.SerializeObject(param)}| {ex.Message}");
            }
            return null;
        }

        public async Task<CompanyProfileDataModel> GetCompanyInfo(string code)
        {
            try
            {
                var url = string.Format(ServiceSetting._companyInfo, code);
                var client = new HttpClient { BaseAddress = new Uri(url) };
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<CompanyProfileModel>(resultArray);
                return res.data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataAPIService.GetInfoCompany|EXCEPTION|INPUT: {code}| {ex.Message}");
            }
            return null;
        }

        public async Task<List<ShareHolderDataModel>> GetShareHolderCompany(string code)
        {
            try
            {
                var url = string.Format(ServiceSetting._shareHolder, code);
                var client = new HttpClient { BaseAddress = new Uri(url) };
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ShareHolderModel>(resultArray);
                return res.data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataAPIService.GetShareHolderCompany|EXCEPTION|INPUT: {code}| {ex.Message}");
            }
            return null;
        }

        public async Task<ForeignModel> GetForeign(string code, int page, int pageSize, string fromDate, string toDate)
        {
            try
            {
                var url = string.Format(ServiceSetting._foreignBuySell, code, page, pageSize, UrlEncoder.Default.Encode(fromDate), UrlEncoder.Default.Encode(toDate));
                var client = new HttpClient { BaseAddress = new Uri(url) };
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultArray = await responseMessage.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<ForeignModel>(resultArray);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataAPIService.GetForeign|EXCEPTION|INPUT: {code}| {ex.Message}");
            }
            return null;
        }

        public async Task<List<Quote>> GetDataStock(string code)
        {
            var lOutput = new List<Quote>();
            try
            {
                var url = string.Format(ServiceSetting._stockData, code, "1D", DateTimeOffset.Now.AddYears(-1).ToUnixTimeSeconds(), DateTimeOffset.Now.ToUnixTimeSeconds());
                var client = new HttpClient { BaseAddress = new Uri(url) };
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

        public async Task<Stream> GetTuDoanhHNX(EHnxExchange mode)
        {
            var lOutput = new List<Quote>();
            try
            {
                var dt = DateTime.Now;
                var strDate = $"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}";
                var url = string.Format(ServiceSetting._tudoanhHNX, strDate, mode.ToString());
                var client = new HttpClient { BaseAddress = new Uri(url) };
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
                var client = new HttpClient { BaseAddress = new Uri(url) };
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var html = await responseMessage.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"body\"]/div[2]/div[1]/div[2]/div[1]/div/div[2]/div/div[2]/ul") as IEnumerable<HtmlNode>;
                foreach (HtmlNode node in nodes.ElementAt(0).ChildNodes)
                {
                    if (string.IsNullOrWhiteSpace(node.InnerText))
                        continue;

                    var document = new HtmlDocument();
                    document.LoadHtml(node.InnerHtml);
                    var tagA = document.DocumentNode.SelectSingleNode("//a");
                    var title = tagA.Attributes["title"].Value;
                    if (!(title.Contains("giao dịch tự doanh")
                        && title.Contains($"{dt.Day.To2Digit()}/{dt.Month.To2Digit()}/{dt.Year}")))
                        continue;

                    link = tagA.Attributes["href"].Value;
                    break;
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
    }
}
