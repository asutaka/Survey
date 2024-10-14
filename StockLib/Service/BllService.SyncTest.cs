using StockLib.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task SyncTest()
        {
            //var lData = _daukhiRepo.GetAll();
            //foreach (var item in lData)
            //{
            //    var entity = new Financial
            //    {
            //        s = item.s,
            //        d = item.d,
            //        t = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
            //        pl = item.pl,
            //        rv = item.rv,
            //        pf = item.pf,
            //        pfg = item.pfg,
            //        pfn = item.pfn,
            //        inv = item.inv,
            //        debt = item.debt,
            //        eq = item.eq,
            //    };
            //    _financialRepo.InsertOne(entity);
            //}
        }
    }
}