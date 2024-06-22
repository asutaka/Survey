using Newtonsoft.Json;
using SurveyStock.Model.APIModel;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SurveyStock.Service
{
    public interface IDataAPIService
    {
        Task<List<string>> GetStock(EStockExchange param);
        Task<CompanyProfileDataModel> GetCompanyInfo(string code);
        Task<List<ShareHolderDataModel>> GetShareHolderCompany(string code);

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
    }
}
