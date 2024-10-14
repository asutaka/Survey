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
            //var lData = _nhRepo.GetAll();
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
            //        //inv = item.inv,
            //        debt = item.debt,
            //        risk = item.risk,
            //        nim_r = item.nim_r,
            //        casa_r = item.casa_r,
            //        cir_r = item.cir_r,
            //        credit_r = item.credit_r,
            //        cost_r = item.cost_r,
            //        debt1 = item.debt1,
            //        debt2 = item.debt2,
            //        debt3 = item.debt3,
            //        debt4 = item.debt4,
            //        debt5 = item.debt5,
            //        cover_r = item.cover_r,
            //        room = item.room
            //    };
            //    _financialRepo.InsertOne(entity);
            //}
        }
    }
}