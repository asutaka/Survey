using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.TestData
{
    public class Test08122023
    {
        public static void Test1()
        {
            var lData = Data.GetData("axsusdt", EInterval.I15M);
            var count = lData.Count();
            //Tính MA20 volumne
            var lMa20 = CalculateMng.MA(lData.Select(x => x.Volume).ToArray(), TicTacTec.TA.Library.Core.MAType.Sma, 20, count);
            //Lấy những giá trị mà có vol = 2 vol trước đó và > Ma20
            var lResult = new List<FinancialDataPoint>();
            FinancialDataPoint prevData = null;
            for (int i = 0; i < count; i++)
            {
                var item = lData.ElementAt(i);
                if (prevData != null)
                {
                    if (item.Close > item.Open && item.Volume > prevData.Volume * 2)
                    {
                        if(i>= 20 && item.Volume > lMa20.ElementAt(i - 20))
                        {
                            lResult.Add(item);
                        }
                        
                    }
                }
                prevData = item;
            }
            
            var tmp9 = 1;
        }
    }
}
