using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using SLib.DAL;
using SLib.Google;
using SLib.Model.GoogleSheetModel;
using System.Collections.Generic;
using System.Linq;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace SLib.Service
{
    public interface IGoogleService
    {
        int GGDoanhThu(string nhomNganh);
        int GGLoiNhuan(string nhomNganh);
        void GGLoadData();
        //List<Item> Get();
        //Item GetByRow(int rowId);
        //void Post(Item item);
        //void Put(int rowId, Item item);
        //void Delete(int rowId);
    }
    public partial class GoogleService : IGoogleService
    {
        //const string SPREADSHEET_ID = "1quPlghuiQoLaQWuI7kJP0KsAjYBlFkg8Gnrvkfg7Hgk";
        //const string SHEET_NAME = "DoanhThu";
        private readonly SpreadsheetsResource.ValuesResource _googleSheetValues;
        private readonly IGoogleSheetRepo _googleRepo;
        private readonly IGoogleDataRepo _ggDataRepo;
        private readonly IFinancialRepo _financialRepo;
        private readonly IStockRepo _stockRepo;

        public GoogleService(GoogleSheetsHelper googleSheetsHelper,
                            IGoogleSheetRepo googleRepo,
                            IGoogleDataRepo ggDataRepo,
                            IStockRepo stockRepo,
                            IFinancialRepo financialRepo) 
        {
            _googleSheetValues = googleSheetsHelper.Service.Spreadsheets.Values;
            _googleRepo = googleRepo;
            _ggDataRepo = ggDataRepo;
            _financialRepo = financialRepo;
            _stockRepo = stockRepo;
        }

        //public List<Item> Get()
        //{
        //    var range = $"{SHEET_NAME}!A:D";
        //    var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
        //    var response = request.Execute();
        //    var values = response.Values;
        //    var res = ItemsMapper.MapFromRangeData(values);
        //    return res;
        //}

        //public Item GetByRow(int rowId)
        //{
        //    var range = $"{SHEET_NAME}!A{rowId}:D{rowId}";
        //    var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
        //    var response = request.Execute();
        //    var values = response.Values;
        //    var res = ItemsMapper.MapFromRangeData(values).FirstOrDefault();
        //    return res;
        //}

        //public void Post(Item item)
        //{
        //    var range = $"{SHEET_NAME}!A:D";
        //    var valueRange = new ValueRange
        //    {
        //        Values = ItemsMapper.MapToRangeData(item)
        //    };
        //    var appendRequest = _googleSheetValues.Append(valueRange, SPREADSHEET_ID, range);
        //    appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
        //    appendRequest.Execute();
        //}

        public void Put(int rowId, GoogleDataSheet item, string spreadID, string sheetName, string range)
        {
            //var range = $"{sheetName}!B{rowId}:Z{rowId}";
            var valueRange = new ValueRange
            {
                Values = item.MapToRangeData()
            };
            var updateRequest = _googleSheetValues.Update(valueRange, spreadID, range);
            updateRequest.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
            updateRequest.Execute();
        }

        //public void Delete(int rowId)
        //{
        //    var range = $"{SHEET_NAME}!A{rowId}:D{rowId}";
        //    var requestBody = new ClearValuesRequest();
        //    var deleteRequest = _googleSheetValues.Clear(requestBody, SPREADSHEET_ID, range);
        //    deleteRequest.Execute();
        //}
    }

    public static class Mapper
    {
        public static List<GoogleDataSheet> MapFromRangeData(this IList<IList<object>> values)
        {
            var items = new List<GoogleDataSheet>();
            foreach (var value in values)
            {
                var item = new GoogleDataSheet
                {
                    lValues = new List<string>()
                };
                if (!value.Any())
                {
                    items.Add(item);
                    continue;
                }
                var count = value.Count();
                for (int i = 0; i < count; i++)
                {
                    if (i == 0)
                    {
                        item.Code = value[0].ToString().Trim();
                        continue;
                    }
                    item.lValues.Add(value[i].ToString().Trim());
                }
                items.Add(item);
            }
            return items;
        }

        public static IList<IList<object>> MapToRangeData(this GoogleDataSheet item)
        {
            var objectList = new List<object>();
            objectList.AddRange(item.lValues);
            var rangeData = new List<IList<object>> { objectList };
            return rangeData;
        }
    }
}
