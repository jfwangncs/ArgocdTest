using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArgoTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        ILogger<WeatherForecast> _logger;

        public WeatherForecastController(ILogger<WeatherForecast> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<String> Get()
        {
            var version = "V23";
            var startTime = DateTime.UtcNow;
            await Task.Delay(new Random().Next(1000));
            var endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalMilliseconds;
            _logger.LogInformation("Version:{Version},Log:{Log},Duration:{Duration}", version, "测试", duration);
            return version;

        }
    }
}
