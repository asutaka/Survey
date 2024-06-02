using SurveyStock.BLL;
using SurveyStock.DAL;
using SurveyStock.Util;
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
            //Đồng bộ bảng
            //WebHandle.SyncTable();

            //var dt = DateTime.Now;
            //if (dt.Hour <= 8 || dt.Hour >= 15)
            //{
            //    WebHandle.SyncDataDay();
            //}

            //CalMng.BuySellByMa20("BCG");
            //Tính tỉ lệ TP rank cổ phiếu
            //CalMng.Ma20RateAboveBelow_1d();

            //Survey 1 cổ phiếu
            //CalMng.BuySellByMa("dpg");
            CalMng.RankStock(EmaType.MA, 20);
            //var lst = new List<string>
            //                        {
            //                            "PYU",
            //                            "ROS"
            //                        };

            //foreach (var item in lst)
            //{
            //    sqliteComDB.UpdateStatus_Company(item, false);
            //}
        }
    }
}
