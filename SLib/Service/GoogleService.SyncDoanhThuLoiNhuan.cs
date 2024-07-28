using MongoDB.Driver;
using SLib.Model;
using SLib.Model.GoogleSheetModel;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace SLib.Service
{
    public partial class GoogleService 
    {
        private List<GoogleDataSheet> GetGoogleData(string spreadId, string sheetName)
        {
            var range = $"{sheetName}!A:Z";
            var request = _googleSheetValues.Get(spreadId, range);
            var response = request.Execute();
            var values = response.Values;
            return values.MapFromRangeData();
        }
        public int GGDoanhThu(string nhomNganh)
        {
            try
            {
                 var sheetName = "DoanhThu";
                var lGoogle = _googleRepo.GetAll();
                var entityNganHang = lGoogle.FirstOrDefault(x => x.fn == nhomNganh);
                if (entityNganHang is null)
                    return -1;

                var lRecord = GetGoogleData(entityNganHang.ssi, sheetName);
                var entityTitle = lRecord.First();
                lRecord.RemoveAt(0);
                var rowIndex = 1;
                foreach ( var row in lRecord)
                {
                    rowIndex++;
                    if (string.IsNullOrWhiteSpace(row.Code))
                        continue;

                    var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, row.Code));
                    lFinancial = lFinancial?.Where(x => x.lengthReport > 0 && x.lengthReport < 5).OrderByDescending(x => x.d).ToList();
                    if (lFinancial is null)
                        continue;

                    var updateOn = row.lValues.Count();
                    row.lValues.Clear();
                    var demTitle = 0;
                    foreach (var itemTitle in entityTitle.lValues)
                    {
                        try
                        {
                            demTitle++;
                            if (demTitle <= updateOn)
                                continue;
                            if (string.IsNullOrWhiteSpace(itemTitle))
                                continue;
                            var mq = itemTitle.Replace("Q", "").Split('/');
                            var quarter = int.Parse(mq[0]);
                            var year = int.Parse(mq[1]);
                            var entityFinancial = lFinancial.FirstOrDefault(x => x.yearReport == year && x.lengthReport == quarter);
                            if (entityFinancial is null)
                            {
                                row.lValues.Add(string.Empty);
                                continue;
                            }
                            if(rowIndex >= 50)
                            {
                                row.lValues.Add(entityFinancial.revenue.ToString());
                            }
                            else
                            {
                                //tỉ lệ
                                var entityFinancialPrev = lFinancial.FirstOrDefault(x => x.yearReport == year - 1 && x.lengthReport == quarter);
                                if(entityFinancialPrev is null)
                                {
                                    row.lValues.Add(string.Empty);
                                    continue;
                                }
                                var rate = Math.Round(100 * (-1 + (entityFinancial.revenue ?? 0) / (entityFinancialPrev.revenue ?? 1)), 1);
                                row.lValues.Add($"{rate}");
                            }
                        }
                        catch (Exception ex) 
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (!row.lValues.Any())
                        continue;

                    updateOn = updateOn + 2;//do tính cả cột mã đầu tiên
                    var lastColUpdate = updateOn + row.lValues.Count() - 1;
                    var rangeUpdate = $"{sheetName}!{(ESheetCol)updateOn}{rowIndex}:{(ESheetCol)lastColUpdate}{rowIndex}";
                    //
                    Put(rowIndex, row, entityNganHang.ssi, sheetName, rangeUpdate);
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return -1;
        }

        public int GGLoiNhuan(string nhomNganh)
        {
            try
            {
                var sheetName = "LoiNhuan";
                var lGoogle = _googleRepo.GetAll();
                var entityNganHang = lGoogle.FirstOrDefault(x => x.fn == nhomNganh);
                if (entityNganHang is null)
                    return -1;

                var lRecord = GetGoogleData(entityNganHang.ssi, sheetName);
                var entityTitle = lRecord.First();
                lRecord.RemoveAt(0);
                var rowIndex = 1;
                foreach (var row in lRecord)
                {
                    rowIndex++;
                    if (string.IsNullOrWhiteSpace(row.Code))
                        continue;

                    var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, row.Code));
                    lFinancial = lFinancial?.Where(x => x.lengthReport > 0 && x.lengthReport < 5).OrderByDescending(x => x.d).ToList();
                    if (lFinancial is null)
                        continue;

                    var updateOn = row.lValues.Count();
                    row.lValues.Clear();
                    var demTitle = 0;
                    foreach (var itemTitle in entityTitle.lValues)
                    {
                        try
                        {
                            demTitle++;
                            if (demTitle <= updateOn)
                                continue;
                            if (string.IsNullOrWhiteSpace(itemTitle))
                                continue;
                            var mq = itemTitle.Replace("Q", "").Split('/');
                            var quarter = int.Parse(mq[0]);
                            var year = int.Parse(mq[1]);
                            var entityFinancial = lFinancial.FirstOrDefault(x => x.yearReport == year && x.lengthReport == quarter);
                            if (entityFinancial is null)
                            {
                                row.lValues.Add(string.Empty);
                                continue;
                            }
                            if (rowIndex >= 50)
                            {
                                row.lValues.Add(entityFinancial.profit.ToString());
                            }
                            else
                            {
                                //tỉ lệ
                                var entityFinancialPrev = lFinancial.FirstOrDefault(x => x.yearReport == year - 1 && x.lengthReport == quarter);
                                if (entityFinancialPrev is null)
                                {
                                    row.lValues.Add(string.Empty);
                                    continue;
                                }
                                var rate = Math.Round(100 * (-1 + (entityFinancial.profit ?? 0) / (entityFinancialPrev.profit ?? 1)), 1);
                                row.lValues.Add($"{rate}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (!row.lValues.Any())
                        continue;

                    updateOn = updateOn + 2;//do tính cả cột mã đầu tiên
                    var lastColUpdate = updateOn + row.lValues.Count() - 1;
                    var rangeUpdate = $"{sheetName}!{(ESheetCol)updateOn}{rowIndex}:{(ESheetCol)lastColUpdate}{rowIndex}";
                    //
                    Put(rowIndex, row, entityNganHang.ssi, sheetName, rangeUpdate);
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }

        public void GGLoadData()
        {
            try
            {
                var lData = new List<GoogleValueModel>();
                var lGoogle = _googleRepo.GetAll();
                foreach (var item in lGoogle)
                {
                    foreach (var itemSheetName in item.sh)
                    {
                        try
                        {
                            var lRecord = GetGoogleData(item.ssi, itemSheetName);
                            var entityTitle = lRecord.First();
                            lRecord.RemoveAt(0);
                            var rowIndex = 1;
                            foreach (var row in lRecord)
                            {
                                rowIndex++;
                                try
                                {
                                    if (string.IsNullOrWhiteSpace(row.Code))
                                        continue;

                                    var lCoin = _stockRepo.GetByFilter(Builders<Stock>.Filter.Eq(x => x.s, row.Code));
                                    if (lCoin is null
                                        || !lCoin.Any())
                                        continue;

                                    var count = row.lValues.Count();
                                    for (int i = 0; i < count; i++)
                                    {
                                        var itemTitle = entityTitle.lValues.ElementAt(i);
                                        itemTitle = itemTitle.Replace("Q", "");
                                        var strQY = itemTitle.Split("/");
                                        var ggModel = new GoogleValueModel
                                        {
                                            NhomNganh = item.fn,
                                            SheetName = itemSheetName,
                                            Code = row.Code,
                                            Quarter = int.Parse(strQY[0]),
                                            Year = int.Parse(strQY[1])
                                        };
                                        if (rowIndex >= 50)
                                        {
                                            ggModel.Type = 0;
                                            var isDecimal = decimal.TryParse(row.lValues[i].Replace(".","").Replace(",", "."), out var val);
                                            if (isDecimal)
                                                ggModel.Value = val;
                                        }
                                        else
                                        {
                                            ggModel.Type = 1;
                                            var isDecimal = decimal.TryParse(row.lValues[i].Replace(",", "."), out var val);
                                            if (isDecimal)
                                                ggModel.Rate = val;
                                        }
                                        lData.Add(ggModel);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        catch(Exception ex)
                        { 
                            Console.WriteLine($"INPUT: {item.fn}/{itemSheetName}|{ex.Message}");
                        }
                    }
                }
                _ggDataRepo.DeleteMany(Builders<GoogleData>.Filter.Gte(x => x.t, 0));
                foreach (var item in lData)
                {
                    _ggDataRepo.InsertOne(new GoogleData
                    {
                        t = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        ty = item.Type,
                        nhom = item.NhomNganh,
                        sheet = item.SheetName,
                        code = item.Code,
                        year = item.Year,
                        quarter = item.Quarter,
                        value = item.Value,
                        rate = item.Rate
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

   
}
