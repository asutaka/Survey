using StockLib.Service;

namespace StockLib.PublicService
{
    public interface IStockTestCaseService
    {
        Task SurveySuperTrend(string code);
        Task SurveySuperTrendPhrase2(string code);
        Task SurveyGoldFish(string code);
        Task SurveyVCP(string code);
        Task SurveyW(string code);
        Task SurveyDanZagerCustom(string code);
        Task SurveyMa20(string code);
        Task Survey3C(string code);
        Task SurveyT3(string code);
        void RankChungKhoan();
        void TotalDays();
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

        public async Task SurveySuperTrendPhrase2(string code)
        {
            await _partternService.SurveySuperTrendPhrase2(code);
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

        public async Task SurveyMa20(string code)
        {
            await _partternService.SurveyMa20(code);
        }

        public async Task SurveyT3(string code)
        {
            await _partternService.SurveyT3(code);
        }

        public async Task SurveyDanZagerCustom(string code)
        {
            await _partternService.SurveyDanZagerCustom(code);
        }

        public async Task Survey3C(string code)
        {
            await _partternService.Survey3C(code);
        }

        public void RankChungKhoan()
        {
            _partternService.RankChungKhoan();
        }

        public void TotalDays()
        {
            _partternService.TotalDays();
        }
    }
}
