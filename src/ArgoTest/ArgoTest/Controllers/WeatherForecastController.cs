using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            var version = "V26";
            var startTime = DateTime.UtcNow;
            await Task.Delay(new Random().Next(1000));
            var endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalMilliseconds;
            var org = new Organization() { Name = "RG", Tag = new List<string>() { "a", "B", "D" } };
            var user = new User()
            {
                Id = 23,
                Name = "希望",
                Organization = org,
                Team = new List<Team>() { new Team { Organization = org, Tag = ["x", "y", "z"] },
                new Team { Organization = new Organization() { Name = "WB",Tag = new List<string>() { "o", "跑", "大打" } }, Tag = ["！@#", "大家", "大打"] } }
            };
            _logger.LogInformation("Version:{Version},Log:{Log},Duration:{Duration},User:{@User}", version, "测试", duration, user);
            return version;

        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Organization Organization { get; set; }
        public List<Team> Team { get; set; }
    }

    public class Organization
    {
        public string Name { get; set; }

        public List<string> Tag { get; set; }
    }
    public class Team
    {
        public Organization Organization { get; set; }
        public List<string> Tag { get; set; }
    }
}
