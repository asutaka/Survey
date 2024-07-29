using SLib.Model.APIModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLib.Service
{
    public partial class BllService
    {
        private decimal LoiNhuanDN_TB(KeHoachThucHienAPIModel KeHoach)
        {
            var dt = DateTime.Now;
            if (!KeHoach.data.Any())
                return 0;
            KeHoach.data = KeHoach.data.Where(x => x.year >= dt.Year - 5).ToList();
            var lKetquaNam = KeHoach.data.OrderByDescending(x => x.year).Select(x => x.quarter.FirstOrDefault(y => y.quarter == 0)).Where(x => x != null);
            var lLoiNhuan = new List<decimal>();
            var count = lKetquaNam.Count();
            if (count > 1)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var itemCur = lKetquaNam.ElementAt(i).isa22_report;
                    var itemPrev = lKetquaNam.ElementAt(i + 1).isa22_report;

                    var loinhuanTB = Math.Abs(Math.Round(100 * (-1 + itemCur / itemPrev)));
                    loinhuanTB = itemCur >= itemPrev ? loinhuanTB : -loinhuanTB;
                    lLoiNhuan.Add(loinhuanTB);
                }
            }
            if (!lLoiNhuan.Any())
                return 0;
            return lLoiNhuan.Average();
        }
    }
}
