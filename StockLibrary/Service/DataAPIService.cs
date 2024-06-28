using Newtonsoft.Json;
using Skender.Stock.Indicators;
using StockLibrary.Model.APIModel;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace StockLibrary.Service
{
    public interface IDataAPIService
    {
        Task<List<string>> GetStock(EStockExchange param);
        Task<CompanyProfileDataModel> GetCompanyInfo(string code);
        Task<List<ShareHolderDataModel>> GetShareHolderCompany(string code);
        Task<ForeignModel> GetForeign(string code, int page, int pageSize, string fromDate, string toDate);
        Task<List<Quote>> GetDataStock(string code);

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
                var responseModel = JsonConvert.DeserializeObject<StockDataModel>(resultArray);
                if (responseModel.t.Any())
                {
                    var count = responseModel.t.Count();
                    for (int i = 0; i < count; i++)
                    {
                        lOutput.Add(new Quote
                        {
                            Date = responseModel.t.ElementAt(i).UnixTimeStampToDateTime(),
                            Open = responseModel.o.ElementAt(i),
                            Close = responseModel.c.ElementAt(i),
                            High = responseModel.h.ElementAt(i),
                            Low = responseModel.l.ElementAt(i),
                            Volume = responseModel.v.ElementAt(i)
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
    }
}
