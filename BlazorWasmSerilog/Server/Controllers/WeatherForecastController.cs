using BlazorWasmSerilog.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlazorWasmSerilog.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

   

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILoggerFactory LoggerFactory)
        {
            _logger = logger;
            var logger2 = LoggerFactory.CreateLogger<WeatherForecastController>();
            logger2.LogWarning("Testing Logger Factory");

        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogWarning("Server Side Warning");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}