
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace CacheTester.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDatabase _db;


    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
        _db = redis.GetDatabase();
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    [HttpGet]
    public async Task<IActionResult> GetItemByIndexFromDb(int index)
    {
        var items = _db.HashValues($"doc:{index}");
        return Ok(items.Select(item => item.ToString()));
    }
}