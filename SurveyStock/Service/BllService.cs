using SurveyStock.DAL.MongoDAL;
using SurveyStock.Model.APIModel;
using SurveyStock.Model.MongoModel;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.Service
{
    public interface IBllService
    {
        Task SyncCompany();
        Task InsertTransaction(List<Transaction> lInput);
    }
    public class BllService : IBllService
    {
        private readonly IDataAPIService _dataService;
        private readonly IStockMongoRepo _stockRepo;
        private readonly ITransactionMongoRepo _transRepo;
        public BllService(IDataAPIService dataService,
                            IStockMongoRepo stockRepo,
                            ITransactionMongoRepo transRepo
                            )
        {
            _dataService = dataService;
            _stockRepo = stockRepo;
            _transRepo = transRepo;
        }
        public async Task SyncCompany()
        {
            var lCompany = await _stockRepo.GetAllAsync();

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

        public async Task InsertTransaction(List<Transaction> lInput)
        {
            foreach (var item in lInput)
            {
                var tmp = await _stockRepo.GetAllAsync();
                //Check Exists
                var lFind = await _transRepo.GetWithFilterAsync(1, 20, item.ma_ck, item.ngay, item.type);
                if ((lFind?? new List<Transaction>()).Any())
                    continue;
                item.create_at = DateTime.Now;
                await _transRepo.InsertOneAsync(item);
            }
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
                await _stockRepo.InsertOneAsync(model);
            }
        }
    }
}
