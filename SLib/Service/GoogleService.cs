using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using SLib.Google;
using SLib.Model.GoogleSheet;
using SLib.Util;
using System.Collections.Generic;
using System.Linq;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace SLib.Service
{
    public interface IGoogleService
    {
        List<Item> Get();
        Item GetByRow(int rowId);
        void Post(Item item);
        void Put(int rowId, Item item);
        void Delete(int rowId);
    }
    public class GoogleService : IGoogleService
    {
        const string SPREADSHEET_ID = "1quPlghuiQoLaQWuI7kJP0KsAjYBlFkg8Gnrvkfg7Hgk";
        const string SHEET_NAME = "DoanhThu";
        SpreadsheetsResource.ValuesResource _googleSheetValues;
        public GoogleService(GoogleSheetsHelper googleSheetsHelper) 
        {
            _googleSheetValues = googleSheetsHelper.Service.Spreadsheets.Values;
        }

        public List<Item> Get()
        {
            var range = $"{SHEET_NAME}!A:D";
            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            var response = request.Execute();
            var values = response.Values;
            var res = ItemsMapper.MapFromRangeData(values);
            return res;
        }

        public Item GetByRow(int rowId)
        {
            var range = $"{SHEET_NAME}!A{rowId}:D{rowId}";
            var request = _googleSheetValues.Get(SPREADSHEET_ID, range);
            var response = request.Execute();
            var values = response.Values;
            var res = ItemsMapper.MapFromRangeData(values).FirstOrDefault();
            return res;
        }

        public void Post(Item item)
        {
            var range = $"{SHEET_NAME}!A:D";
            var valueRange = new ValueRange
            {
                Values = ItemsMapper.MapToRangeData(item)
            };
            var appendRequest = _googleSheetValues.Append(valueRange, SPREADSHEET_ID, range);
            appendRequest.ValueInputOption = AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.Execute();
        }

        public void Put(int rowId, Item item)
        {
            var range = $"{SHEET_NAME}!A{rowId}:D{rowId}";
            var valueRange = new ValueRange
            {
                Values = ItemsMapper.MapToRangeData(item)
            };
            var updateRequest = _googleSheetValues.Update(valueRange, SPREADSHEET_ID, range);
            updateRequest.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;
            updateRequest.Execute();
        }

        public void Delete(int rowId)
        {
            var range = $"{SHEET_NAME}!A{rowId}:D{rowId}";
            var requestBody = new ClearValuesRequest();
            var deleteRequest = _googleSheetValues.Clear(requestBody, SPREADSHEET_ID, range);
            deleteRequest.Execute();
        }
    }
}
