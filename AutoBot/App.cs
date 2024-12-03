using Microsoft.Extensions.Logging;
using StockLib.PublicService;
using StockLib.Service;

namespace AutoBot
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IAutobotService _service;
        private readonly IBllService _bllService;

        public App(ILogger<App> logger,
                        IAutobotService service,
                        IBllService bllService)
        {
            _logger = logger;
            _service = service;
            _bllService = bllService;
        }

        public async Task Run(string[] args)
        {
            await _service.LiquidTrace();
        }
    }
}
