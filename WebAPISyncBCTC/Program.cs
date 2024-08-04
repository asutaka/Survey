using WebAPISyncBCTC;
using SLib.Service;
using SLib.Google;

WebApplicationOptions options = new()
{
    ContentRootPath = AppContext.BaseDirectory,
    Args = args
};
var builder = WebApplication.CreateBuilder(options);

builder.Host.UseWindowsService();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient();
builder.Services.AddSLib();
builder.Services.AddSingleton(typeof(GoogleSheetsHelper));
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AllowSynchronousIO = true;
});
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
})
.WithName("GetWeatherForecast");

app.MapPost("/upload",
    async (HttpRequest request, IBllService service) =>
    {
        await service.BCTCImport(request.Body);
        return "File Was Processed Sucessfully!";
    }).Accepts<IFormFile>("multipart/form-data");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}