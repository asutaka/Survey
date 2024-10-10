using Microsoft.Extensions.Logging;
using StockLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.PublicService
{
    public interface IAnalyzeCoinService
    {
        Task AnalyzeJob();
    }
    public class AnalyzeCoinService : IAnalyzeCoinService
    {
        private readonly ILogger<AnalyzeCoinService> _logger;
        private readonly ITeleService _teleService;
        private readonly IAnalyzeService _analyzeService;
        private const long _idUser = 1066022551;
        public AnalyzeCoinService(ILogger<AnalyzeCoinService> logger,
                                    ITeleService teleService,       
                                    IAnalyzeService analyzeService)
        {
            _logger = logger;
            _analyzeService = analyzeService;
            _teleService = teleService;
        }

        public async Task AnalyzeJob()
        {
            try
            {
                var tinhieu = await _analyzeService.TinHieuMuaBanCoin();
                if (tinhieu.Item1 > 0)
                {
                    await _teleService.SendTextMessageAsync(_idUser, tinhieu.Item2);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeCoinService.AnalyzeJob|EXCEPTION| {ex.Message}");
            }
        }
    }
}
