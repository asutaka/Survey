using MongoDB.Driver;
using StockLibrary.DAL;
using StockLibrary.Mapping;
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
        private readonly ITuDoanhMongoRepo _tudoanhRepo;
        private readonly IForeignMongoRepo _foreignRepo;
        private readonly IReportMongoRepo _reportRepo;
        public BllService(IDataAPIService dataService,
                            IStockMongoRepo stockRepo,
                            ITuDoanhMongoRepo tudoanhRepo,
                            IReportMongoRepo reportRepo,
                            IForeignMongoRepo foreignRepo
                            )
        {
            _dataService = dataService;
            _stockRepo = stockRepo;
            _tudoanhRepo = tudoanhRepo;
            _foreignRepo = foreignRepo;
            _reportRepo = reportRepo;
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
                var lFind = _tudoanhRepo.GetWithFilter(1, 20, item.ma_ck, item.d);
                if ((lFind?? new List<TuDoanh>()).Any())
                    continue;
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
            var dt = DateTime.Now;
            if (dt.DayOfWeek == DayOfWeek.Saturday
                || dt.DayOfWeek == DayOfWeek.Sunday
                || dt.Hour < 17)
                return;

            var lStock = _stockRepo.GetAll();
            var date = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, TimeSpan.FromHours(0)).ToUnixTimeSeconds();
            var lForeign = _foreignRepo.GetWithFilter(1, 1, "", date);
            var flag = string.Empty;
            if(lForeign != null
                && lForeign.Any())
            {
                flag = lForeign.ElementAt(0).s;
            }

            if(!string.IsNullOrWhiteSpace(flag))
            {
                var lComplete = new List<string>();
                foreach (var itemStock in lStock)
                {
                    lComplete.Add(itemStock.MaCK);
                    if (itemStock.MaCK.Equals(flag))
                    {
                        break;
                    }
                }
                lStock = lStock.Where(x => !lComplete.Any(y => y == x.MaCK)).ToList();
            }

            foreach (var item in lStock)
            {
                Thread.Sleep(1000);
                var foreignResult = await _dataService.GetForeign(item.MaCK, 1, 3, dt.ToString("dd/MM/yyyy"), dt.ToString("dd/MM/yyyy"));
                if (foreignResult is null || foreignResult.data is null)
                    continue;

                InsertGDNuocNgoai(foreignResult.ToForeign());
            }
        }

        private int InsertGDNuocNgoai(List<Foreign> lInput)
        {
            var count = 0;
            foreach (var item in lInput)
            {
                //Check Exists
                //var lFind = _foreignRepo.GetWithFilter(1, 20, item.s, item.d);
                //if ((lFind ?? new List<Foreign>()).Any())
                //    continue;
                _foreignRepo.InsertOne(item);
                count++;
            }
            return count;
        }
    }
}
