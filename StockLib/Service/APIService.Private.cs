using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class APIService
    {
        #region BCTC
        private async Task<Stream> GetFile(string path)
        {
            try
            {
                var client = _client.CreateClient();
                client.BaseAddress = new Uri(path);
                var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return await responseMessage.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.GetFile|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        private async Task<BCTCFileUploadResponse> UploadFile(Stream stream)
        {
            try
            {
                var client = _client.CreateClient();
                var formContent = new MultipartFormDataContent();
                formContent.Add(new StreamContent(stream), "file", "fileUpload.pdf");
                var response = await client.PostAsync("https://filetools0.pdf24.org/client.php?action=upload", formContent);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var readResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<BCTCFileUploadResponse>>(readResponse);
                    return result.FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.UploadFile|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        private async Task<BCTCOCRResponse> OCRFile(BCTCFileUploadResponse model)
        {
            var body = new BCTCOCRInput
            {
                files = new List<BCTCFileUploadResponse> { model }
            };
            try
            {
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = cookies;

                var url = "https://filetools0.pdf24.org/client.php?action=ocrPdf";
                var client = new HttpClient(handler);
                client.BaseAddress = new Uri(url);
                var requestMessage = new HttpRequestMessage();
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var readResponse = await responseMessage.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<BCTCOCRResponse>(readResponse);

                    Uri uri = new Uri("http://google.com");
                    IEnumerable<Cookie> responseCookies = cookies.GetCookies(client.BaseAddress).Cast<Cookie>();
                    var responseCookie = responseCookies.FirstOrDefault();
                    if (responseCookie != null)
                    {
                        result.cookieName = responseCookie.Name;
                        result.cookieValue = responseCookie.Value;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.OCRFile|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        private async Task<BCTCStatusResponse> GetStatus(BCTCOCRResponse model)
        {
            try
            {
                var url = $"https://filetools0.pdf24.org/client.php?action=getStatus&jobId={model.jobId}";
                using var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", $"{model.cookieName}={model.cookieValue}");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "");
                request.Content = new StringContent("",
                                                    Encoding.UTF8,
                                                    "application/json");
                var response = await client.SendAsync(request);
                var contents = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<BCTCStatusResponse>(contents);
                    result.cookieName = model.cookieName;
                    result.cookieValue = model.cookieValue;
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.GetStatus|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> DownloadFile(BCTCStatusResponse model)
        {
            try
            {
                var dt = DateTime.Now;
                var url = $"https://filetools0.pdf24.org/client.php?mode=download&action=downloadJobResult&jobId={model.jobId}";
                using var client = _client.CreateClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", $"{model.cookieName}={model.cookieValue}");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "");
                request.Content = new StringContent("",
                                                    Encoding.UTF8,
                                                    "application/json");
                var response = await client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    //Save file
                    //Stream streamCopy = null; ;
                    //stream.CopyTo(streamCopy);
                    //var bytesInStream = new byte[streamCopy.Length];
                    //await streamCopy.ReadAsync(bytesInStream, 0, (int)bytesInStream.Length);
                    //using (var outputFileStream = new FileStream($"E:/{Guid.NewGuid()}.pdf", FileMode.Create))
                    //{
                    //    await outputFileStream.WriteAsync(bytesInStream, 0, bytesInStream.Length);
                    //}

                    return stream;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APIService.DownloadFile|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        #endregion
    }
}
