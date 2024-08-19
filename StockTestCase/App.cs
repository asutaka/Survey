using Microsoft.Extensions.Logging;
using StockLib.PublicService;

namespace StockTestCase
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IFinancialDataService _service;

        public App(ILogger<App> logger,
                        IFinancialDataService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task Run(string[] args)
        {
            Console.WriteLine("test");
            Console.ReadLine();
            //await _service.SyncBCTC_BatDongSan();
        }
    }
}
