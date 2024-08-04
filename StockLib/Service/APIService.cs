using Microsoft.Extensions.Logging;
using StockLib.Model;
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
    }
}
