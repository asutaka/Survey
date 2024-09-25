using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IStockTestCaseService
    {
        Task SurveySuperTrend(string code);
        Task SurveyGoldFish(string code);
        Task SurveyVCP(string code);
        Task SurveyW(string code);
        Task SurveyDanZagerCustom(string code);
        void RankChungKhoan();
    }
    public class StockTestCaseService : IStockTestCaseService
    {
        private readonly IPartternService _partternService;
        public StockTestCaseService(IPartternService partternService) 
        {
            _partternService = partternService;
        }

        public async Task SurveySuperTrend(string code)
        {
            await _partternService.SurveySuperTrend(code);
        }

        public async Task SurveyGoldFish(string code)
        {
            await _partternService.SurveyGoldFish(code);
        }

        public async Task SurveyVCP(string code)
        {
            await _partternService.SurveyVCP(code);
        }

        public async Task SurveyW(string code)
        {
            await _partternService.SurveyW(code);
        }

        public async Task SurveyDanZagerCustom(string code)
        {
            await _partternService.SurveyDanZagerCustom(code);
        }

        public void RankChungKhoan()
        {
            _partternService.RankChungKhoan();
        }
    }
}
