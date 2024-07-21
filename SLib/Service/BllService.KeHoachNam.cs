using SLib.Model.APIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        private async Task<decimal> LoiNhuanDN_TB(string code)
        {
            var dt = DateTime.Now;
            var KeHoach = await _apiService.GetKeHoachThucHien(code);
            return LoiNhuanDN_TB(KeHoach);
        }

        private decimal LoiNhuanDN_TB(KeHoachThucHienAPIModel KeHoach)
        {
            var dt = DateTime.Now;
            if (!KeHoach.data.Any())
                return 0;
            KeHoach.data = KeHoach.data.Where(x => x.year < dt.Year).ToList();
            var lKetquaNam = KeHoach.data.Select(x => x.quarter.FirstOrDefault(y => y.quarter == 0));
            var lLoiNhuan = new List<decimal>();
            var count = lKetquaNam.Count();
            if (count > 1)
            {
                for (var i = 0; i < count - 1; i++)
                {
                    var itemCur = lKetquaNam.ElementAt(i).isa16_report;
                    var itemPrev = lKetquaNam.ElementAt(i + 1).isa16_report;



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
