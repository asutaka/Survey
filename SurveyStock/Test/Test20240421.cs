using SurveyStock.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyStock.Test
{
    public static class Test20240421
    {
        public static void MainFunc()
        {
            //var dt = DateTime.Now;
            //if(dt.Hour <= 8 || dt.Hour >= 15)
            //{
            //    WebHandle.SyncDataDay();
            //}

            CalMng.BuySellByMa20("BCG");
            //Tính tỉ lệ TP rank cổ phiếu
            //CalMng.Ma20RateAboveBelow_1d();
        }
    }
}
