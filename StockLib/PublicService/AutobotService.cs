using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IAutobotService
    {
        Task LiquidTrace();
    }
    public class AutobotService : IAutobotService
    {
        private readonly IAnalyzeService _analyzeService;
        public AutobotService(IAnalyzeService analyzeService) 
        {
            _analyzeService = analyzeService;
        }

        public async Task LiquidTrace()
        {
            await _analyzeService.LiquidTrace();
        }
    }
}
