using Survey.Models;
using Survey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.TestData
{
    public class Test14122023
    {
        private IEnumerable<TicketModel> _lCoin = Data.GetCoins(50);
        public void HandleData()
        {
            if (!_lCoin.Any())
                return;

            var lPass = new List<TicketModel>();
            var count = _lCoin.Count();
            for (int i = 0; i < count; i++)
            {
                var entity = _lCoin.ElementAt(i);
                var lData = Data.GetData(entity.symbol.ToLower(), EInterval.I1H);
                var countData = lData.Count();
                if (countData < 15)
                    continue;

                double last = 0;
                for (int j = countData - 1; j > 0; j--)
                {
                    if(last == 0)
                    {
                        last = lData.ElementAt(j).Close;
                        continue;
                    }

                    var element = lData.ElementAt(j).Close;
                    if((element > last * 1.05 && j <= countData - 11)
                        || (element > last * 1.10 && j <= countData - 21))
                    {
                        //loại
                        break;
                    }

                    if (j <= countData - 21)
                    {
                        lPass.Add(entity);
                        break;
                    }    
                }
            }

            var lCoin = Data.GetCoins(50);
        }
    }
}
