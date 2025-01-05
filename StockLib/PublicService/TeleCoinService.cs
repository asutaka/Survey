using StockLib.Service;

namespace StockLib.PublicService
{
    public interface ITeleCoinService
    {
        Task BotSyncUpdate();
        Task SubcribeCoin();
        Task CalculateCoin();
    }
    public class TeleCoinService : ITeleCoinService
    {
        private readonly ITeleService _service;
        private readonly IAnalyzeService _analyzeService;
        public TeleCoinService(ITeleService service, IAnalyzeService analyzeService)
        {
            _service = service;
            _analyzeService = analyzeService;
        }
        public async Task BotSyncUpdate()
        {
            await _service.BotCoinSyncUpdate();
        }

        public async Task SubcribeCoin()
        {
            await _analyzeService.SubcribeCoin();
        }

        public async Task CalculateCoin()
        {
            await _analyzeService.CalculateCoin();
        }
    }
}