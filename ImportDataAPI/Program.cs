using OfficeOpenXml;
using System.Data;
using StockLib.PublicService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddSLib();
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
});

app.MapPost("/upload", async (HttpRequest request, IImportDataAPIService service) =>
{
    return Results.Ok();

    //Do something with the file
    var files = request.Form.Files;
    var name = request.Form["Name"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest();
    if(!files.Any())
        return Results.BadRequest();

    var file = files.First();
    //List<UserModel> users = new List<UserModel>();
    using var stream = file.OpenReadStream();
    var dic = new Dictionary<int, string>();
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    bool isRead = false;
    using (var package = new ExcelPackage(stream))
    {
        //get the first sheet from the excel file
        var sheet = package.Workbook.Worksheets[0];

        //loop all rows in the sheet
        for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
        {
            //loop all columns in a row
            for (int j = sheet.Dimension.Start.Column; j <= sheet.Dimension.End.Column; j++)
            {
                //do something with the current cell value
                var currentCellValue = sheet.Cells[i, j].Value;
                if(currentCellValue != null)
                {
                    var val = currentCellValue.ToString().Trim();
                    AddDic(val, j);
                   
                    if (j == 1 && val.Equals("CASA", StringComparison.OrdinalIgnoreCase))
                    {
                        isRead = true;
                        break;
                    }
                }
            }

            if(isRead)
            {
                //foreach (var item in dic)
                //{
                //    var strSplit = item.Value.ToString().Replace("Q", "").Split('/');
                //    var d = int.Parse($"{strSplit[1]}{strSplit[0]}");
                //    var res = service.GetFinancial_NH(d, name.Trim().ToUpper());
                //    if (res is null)
                //        continue;
                //    var val = sheet.Cells[i, item.Key].Value;
                //    var check = double.TryParse(val?.ToString() ?? string.Empty, out var valDouble);
                //    if (!check)
                //        continue;

                //    res.casa_r = Math.Round(valDouble * 100, 1);
                //    res.t = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                //    service.UpdateFinancial_NH(res);
                //}
                break;
            }
        }
    }

    void AddDic(string val, int index)
    {
        if(val.Equals("Q1/2020", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q2/2020", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q3/2020", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q4/2020", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }

        else if (val.Equals("Q1/2021", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q2/2021", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q3/2021", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q4/2021", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }

        else if (val.Equals("Q1/2022", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q2/2022", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q3/2022", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q4/2022", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }

        else if (val.Equals("Q1/2023", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q2/2023", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q3/2023", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q4/2023", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }

        else if (val.Equals("Q1/2024", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
        else if (val.Equals("Q2/2024", StringComparison.OrdinalIgnoreCase))
        {
            var check = dic.FirstOrDefault(x => x.Value == val);
            if (check.Value is not null)
                return;
            dic.Add(index, val);
        }
    }

    return Results.Ok();
}).Accepts<IFormFile>("multipart/form-data");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

