using StockLibrary.DAL;
using StockLibrary.Model;
using StockLibrary.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockLibrary.Service
{
    public interface IBllService
    {
        Task<(int, string)> SyncTuDoanhHNX();
        Task<(int, string)> SyncTuDoanhUp();
        Task<(int, List<string>)> SyncTuDoanhHSX();

        Task<(int, List<string>)> SyncGDNuocNgoai();



        Task SyncCompany();

        string TongTuDoanhStr();
        string TongGDNNStr();
        string TuDoanhBuildStr(string code);
        string ForeignBuildStr(string code);
        string ThongKeThiTruongStr();

        void BackgroundWork();
    }
    public partial class BllService : IBllService
    {
        private readonly IDataAPIService _dataService;
        private readonly IStockMongoRepo _stockRepo;
        private readonly IConfigDataMongoRepo _configRepo;
        private readonly ITuDoanhMongoRepo _tudoanhRepo;
        private readonly IForeignMongoRepo _foreignRepo;
        private readonly IReportMongoRepo _reportRepo;
        private readonly IFileService _fileService;
        public BllService(IDataAPIService dataService,
                            IStockMongoRepo stockRepo,
                            IConfigDataMongoRepo configRepo,
                            ITuDoanhMongoRepo tudoanhRepo,
                            IReportMongoRepo reportRepo,
                            IForeignMongoRepo foreignRepo,
                            IFileService fileService
                            )
        {
            _dataService = dataService;
            _stockRepo = stockRepo;
            _configRepo = configRepo;
            _tudoanhRepo = tudoanhRepo;
            _foreignRepo = foreignRepo;
            _reportRepo = reportRepo;
            _fileService = fileService;
        }
        public async Task SyncCompany()
        {
            var lCompany = _stockRepo.GetAll();

            var hose = await _dataService.GetStock(EStockExchange.Hose);
            var lHoseInsert = (hose ?? new List<string>()).Except(lCompany.Select(x => x.MaCK));
            await InsertCompany(lHoseInsert, EStockExchange.Hose);

            var hnx = await _dataService.GetStock(EStockExchange.HNX);
            var lHnxInsert = (hnx ?? new List<string>()).Except(lCompany.Select(x => x.MaCK));
            await InsertCompany(lHnxInsert, EStockExchange.HNX);

            var upcom = await _dataService.GetStock(EStockExchange.Upcom);
            var lUpcomInsert = (upcom ?? new List<string>()).Except(lCompany.Select(x => x.MaCK));
            await InsertCompany(lUpcomInsert, EStockExchange.Upcom);
        }

        private async Task InsertCompany(IEnumerable<string> lInsert, EStockExchange exchangeMode)
        {
            foreach (var itemStock in lInsert)
            {
                if (string.IsNullOrWhiteSpace(itemStock))
                    return;

                var model = new Stock
                {
                    MaCK = itemStock,
                    SanCK = exchangeMode.ToString()
                };
                model.profile = await _dataService.GetCompanyInfo(itemStock);
                model.share_holders = await _dataService.GetShareHolderCompany(itemStock);
                _stockRepo.InsertOne(model);
            }
        }
    }
}
