using MongoDB.Driver;
using SLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<string> ConfigBCTC(string input)
        {
            var strOut = string.Empty;
            try
            {
                input = input.Replace("Config_".ToUpper(), "").Trim();
                var config = _configBCTCRepo.GetAll().FirstOrDefault(x => x.type == 1);
                var data = config.data;
                if (string.IsNullOrWhiteSpace(input))
                {
                    var mes = string.Join("\n", data.Select(x => $"{x.code}({x.name}): {(x.status ? "Enable" : "Disable")}"));
                    strOut = mes;
                }
                else if(input.Contains(",") || int.TryParse(input, out var val))
                {
                    var strSplit = input.Split(',');
                    foreach (var item in data)
                    {
                        item.status = false;
                    }
                    var isUpdate = false;
                    foreach(var item in strSplit) 
                    {
                        var isInt = int.TryParse(item, out var code);
                        if (isInt)
                        {
                            var eData = data.FirstOrDefault(x => x.code == code);
                            if (eData != null)
                            {
                                eData.status = true;
                            }
                            isUpdate = true;
                        }
                    }
                    if(isUpdate)
                    {
                        _configBCTCRepo.UpdateOneField("data", data, Builders<ConfigBCTC>.Filter.Eq(x => x.ObjectId, config.ObjectId));
                        strOut = "Cập nhật cấu hình thành công";
                    }
                    else
                    {
                        strOut = "Không có cấu hình nào phù hợp";
                    }
                }
                else
                {
                    var lStock = _stockRepo.GetByFilter(Builders<Stock>.Filter.Eq(x => x.s, input));
                    if (lStock is null || !lStock.Any())
                    {
                        strOut = "Không tìm thấy cp tương ứng";
                        return strOut;
                    }

                    var stock = lStock.FirstOrDefault();
                    var lBCTC = await _apiService.GetBCTCCafeF(stock.s);
                    if(!lBCTC.Any())
                    {
                        lBCTC = await _apiService.GetBCTCCafeF2(stock.s);
                    }
                    //tinh toan
                    if(lBCTC is null || !lBCTC.Any())
                    {
                        strOut = "Không lấy được dữ liệu BCTC của cp này";
                        return strOut;
                    }

                    foreach (var item in lBCTC)
                    {
                        _fileService.ExtractBCTC(item.Source);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            return strOut;
        }
    }
}
