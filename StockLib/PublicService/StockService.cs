using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IStockService
    {
        Task SyncDataMainBCTCFromWeb();
    }
    public class StockService : IStockService
    {
        private readonly IBllService _bllService;
        public StockService(IBllService bllService) 
        {
            _bllService = bllService;
        }

        public async Task SyncDataMainBCTCFromWeb()
        {
            //await _bllService.SyncDataMainBCTCFromWeb();
        }
    }
}
