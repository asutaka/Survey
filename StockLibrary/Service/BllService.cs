﻿using StockLibrary.DAL;
using StockLibrary.Model;
using StockLibrary.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockLibrary.Service
{
    public interface IBllService
    {
        Task SyncCompany();
        int InsertTransaction(List<Transaction> lInput);
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

        public int InsertTransaction(List<Transaction> lInput)
        {
            var count = 0;
            foreach (var item in lInput)
            {
                //Check Exists
                var lFind = _transRepo.GetWithFilter(1, 20, item.ma_ck, item.ngay, item.type);
                if ((lFind?? new List<Transaction>()).Any())
                    continue;
                item.create_at = DateTime.Now;
                _transRepo.InsertOne(item);
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
    }
}
