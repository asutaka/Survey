using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IStockTestCaseService
    {
        Task SurveyIndicator(string code);
        Task SurveySuperTrend(string code);
        Task GoldFish(string code);
    }
    public class StockTestCaseService : IStockTestCaseService
    {
        private readonly IPartternService _partternService;
        public StockTestCaseService(IPartternService partternService) 
        {
            _partternService = partternService;
        }

        public async Task SurveyIndicator(string code)
        {
            await _partternService.SurveyIchimoku(code);
        }

        public async Task SurveySuperTrend(string code)
        {
            await _partternService.SurveySuperTrend(code);
        }

        public async Task GoldFish(string code)
        {
            await _partternService.GoldFish(code);
        }
    }
}
