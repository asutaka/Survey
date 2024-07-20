using SLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task DongBoNgayCongBoBCTC()
        {
            var lStock = GetStock();
            foreach (var item in lStock)
            {
                try
                {
                    if(item.bc is null)
                    {
                        item.bc = new List<BCTCReleaseModel>();
                    }

                    var lBCTC = await _apiService.GetDanhSachBCTC(item.s);
                    foreach (var itemBC in lBCTC)
                    {
                        if (itemBC.Title.Contains("Thuyết minh"))
                            continue;
                        if(itemBC.Title.Contains("BCTC quý")
                            || itemBC.Title.Contains("BCTC Hợp nhất quý")
                            || itemBC.Title.Contains("BCTC Hợp nhất Soát xét quý"))
                        {
                            var t = itemBC.LastUpdate.Replace("/Date(","").Replace(")/","");
                            var isPass = long.TryParse(t, out var time);
                            if (isPass) 
                            {
                                if (item.bc.Any(x => x.t > time))
                                    continue;

                                item.bc.Add(new BCTCReleaseModel
                                {
                                    t = time,
                                    title = itemBC.Title
                                });
                            }
                        }
                    }

                    _stockRepo.Update(item);
                    //
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
