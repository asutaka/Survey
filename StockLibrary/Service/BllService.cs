using Microsoft.VisualBasic;
using StockLibrary.DAL;
using StockLibrary.Model;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StockLibrary.Service
{
    public interface IBllService
    {
        Task SyncCompany();
        int InsertTuDoanh(List<TuDoanh> lInput);
        Task SyncGDNuocNgoai();
    }
    public class BllService : IBllService
    {
        private readonly IDataAPIService _dataService;
        private readonly IStockMongoRepo _stockRepo;
        private readonly ITuDoanhMongoRepo _tudoanhRepo;
        private readonly IForeignMongoRepo _foreignRepo;
        public BllService(IDataAPIService dataService,
                            IStockMongoRepo stockRepo,
                            ITuDoanhMongoRepo tudoanhRepo,
                            IForeignMongoRepo foreignRepo
                            )
        {
            _dataService = dataService;
            _stockRepo = stockRepo;
            _tudoanhRepo = tudoanhRepo;
            _foreignRepo = foreignRepo;
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

        public int InsertTuDoanh(List<TuDoanh> lInput)
        {
            var count = 0;
            foreach (var item in lInput)
            {
                //Check Exists
                var lFind = _tudoanhRepo.GetWithFilter(1, 20, item.ma_ck, item.ngay, item.type);
                if ((lFind?? new List<TuDoanh>()).Any())
                    continue;
                item.create_at = DateTime.Now;
                _tudoanhRepo.InsertOne(item);
                count++;
            }
            return count;
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

        public async Task SyncGDNuocNgoai()
        {
            var lStock = _stockRepo.GetAll();
            var flag = "REE";
            var lComplete = new List<string>();
            foreach (var itemStock in lStock)
            {
                lComplete.Add(itemStock.MaCK);
                if (itemStock.MaCK.Equals(flag))
                {
                    break;
                }
            }
            
            lStock = lStock.Where(x => !lComplete.Any(y => y == x.MaCK))
                .ToList();
            foreach (var item in lStock)
            {
                var i = 1;
                do
                {
                    Thread.Sleep(1000);
                    var foreignResult = await _dataService.GetForeign(item.MaCK, i, 500, "01/01/2020", "23/06/2024");
                    if (foreignResult is null)
                        continue;

                    InsertGDNuocNgoai(foreignResult.data);
                    var totalRecord = foreignResult.paging.page * foreignResult.paging.pageSize;
                    if (foreignResult.paging.total < totalRecord)
                        break;
                    i++;
                }
                while (true);
            }
        }

        private int InsertGDNuocNgoai(List<Foreign> lInput)
        {
            var count = 0;
            foreach (var item in lInput)
            {
                //Check Exists
                var lFind = _foreignRepo.GetWithFilter(1, 20, item.symbol, item.tradingDate);
                if ((lFind ?? new List<Foreign>()).Any())
                    continue;
                item.create_at = DateTime.Now;
                _foreignRepo.InsertOne(item);
                count++;
            }
            return count;
        }
    }
}
