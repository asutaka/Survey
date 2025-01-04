using StockLib.Service;

namespace StockLib.PublicService
{
    public interface ITeleCoinService
    {
        Task BotSyncUpdate();
        Task SubcribeCoin();
    }
    public class TeleCoinService : ITeleCoinService
    {
        private readonly ITeleService _service;
        public TeleCoinService(ITeleService service)
        {
            _service = service;
        }
        public async Task BotSyncUpdate()
        {
            await _service.BotCoinSyncUpdate();
        }

        public async Task SubcribeCoin()
        {
            await _service.SubcribeCoin();
        }
    }
}