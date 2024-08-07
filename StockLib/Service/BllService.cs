using Microsoft.Extensions.Logging;
using StockLib.DAL;
using StockLib.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace StockLib.Service
{
    public interface IBllService
    {
        Task SyncBCTC_BatDongSan();
        Task SyncBCTC_NganHang();
    }
    public partial class BllService : IBllService
    {
        private readonly ILogger<BllService> _logger;
        private readonly IStockRepo _stockRepo;
        private readonly IStockFinancialRepo _financialRepo;
        private readonly IFinancialBDSRepo _bdsRepo;
        private readonly IFinancialNHRepo _nhRepo;
        private readonly IAPIService _apiService;
        public BllService(ILogger<BllService> logger,
                            IStockRepo stockRepo,
                            IStockFinancialRepo financialRepo,
                            IFinancialBDSRepo financialBDSRepo,
                            IFinancialNHRepo financialNHRepo,
                            IAPIService apiService)
        {
            _logger = logger;
            _stockRepo = stockRepo;
            _financialRepo = financialRepo;
            _bdsRepo = financialBDSRepo;
            _nhRepo = financialNHRepo;
            _apiService = apiService;
        }
    }
}