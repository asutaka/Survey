using Microsoft.Extensions.Logging;
using StockLib.PublicService;

namespace StockTestCase
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IStockTestCaseService _service;

        public App(ILogger<App> logger,
                        IStockTestCaseService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task Run(string[] args)
        {
            await _service.SurveyIndicator("DPG");
        }
    }
}
