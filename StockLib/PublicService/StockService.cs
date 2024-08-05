using Microsoft.Extensions.DependencyInjection;
using StockLib.DAL.Settings;
using StockLib.PublicService.Settings;
using StockLib.Service;
using StockLib.Service.Settings;

namespace StockLib.PublicService
{
    public static class StockDI
    {
        public static void AddSLib(this IServiceCollection services)
        {
            services.DALDependencies();
            services.ServiceDependencies();
            services.PublicServiceDependencies();
        }
    }
    public interface IStockService
    {
        Task SyncDataMainBCTC();
        Task SyncDataMainBCTCFromWeb();
    }
    public class StockService : IStockService
    {
        private readonly IBllService _bllService;
        public StockService(IBllService bllService) 
        {
            _bllService = bllService;
        }
        public async Task SyncDataMainBCTC()
        {
            await _bllService.SyncDataMainBCTC();
        }

        public async Task SyncDataMainBCTCFromWeb()
        {
            await _bllService.SyncDataMainBCTCFromWeb();
        }
    }
}
