using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ExampleCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IMemoryCache _cache;

        private readonly ILogger<WeatherForecastController> _logger;

        //ทำไม Cache ถึงเข้ามาที่นี้ได้ละเพราะว่าเราทำ Dependency Injection ไว้ที่ไฟล์ Startup แล้ว
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache cache)
        {
            _logger = logger;
            //เรียกใช้ Cache
            _cache = cache;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return _cache.GetOrCreate("Weather", entry =>
            {
                // ทดเวลาบาดเจ็บให้ 5 นาทีพอ
                entry.AbsoluteExpiration = DateTime.Now.AddMinutes(5);

                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    // ใส่ with Cache และ Date Time เพื่อให้รู้ว่ามันออกมาจาก Cache จริงนะเออ
                    Summary = $"{Summaries[rng.Next(Summaries.Length)]} with Cache {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}"
                })
              .ToArray();
            });
        }
    }
}
