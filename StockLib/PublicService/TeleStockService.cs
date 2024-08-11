using StockLib.Service;

namespace StockLib.PublicService
{
    public interface ITeleStockService
    {
        Task BotSyncUpdate();
    }
    public class TeleStockService : ITeleStockService
    {
        private readonly ITeleService _service;
        public TeleStockService(ITeleService service)
        {
            _service = service;
        }
        public async Task BotSyncUpdate()
        {
            await _service.BotSyncUpdate();
        }
    }
}
