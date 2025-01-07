using StockLib.Service;

namespace StockLib.PublicService
{
    public interface ITeleCoinService
    {
        Task BotSyncUpdate();
        Task SendMessage(string mes, long id);
        Task SubcribeCoin();
        Task DetectOrderBlock();
        Task<List<string>> CheckEntry();
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

        public async Task SendMessage(string mes, long id)
        {
            await _service.SendTextMessageCoinAsync(id, mes);
        }

        public async Task SubcribeCoin()
        {
            await _analyzeService.SubcribeCoin();
        }

        public async Task DetectOrderBlock()
        {
            await _analyzeService.DetectOrderBlock();
        }

        public async Task<List<string>> CheckEntry()
        {
            return await _analyzeService.CheckEntry();
        }
    }
}