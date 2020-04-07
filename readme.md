# ตัวอย่างการใช้ In-Memory Cache
ติดตั้ง Package ผ่าน CLI หรือ Visual Studio NuGet Package 

```CLI
dotnet add package Microsoft.Extensions.Caching.Memory
```
*ติดตั้งผ่าน dotnet cli*

![Nuget](/images/nuget.png)
*ติดตั้งผ่าน Visual Studio NuGet Package*

เพิ่ม Package ที่ติดตั้งเมื่อกี้เข้ามาใน `Startup.cs`

```csharp
using Microsoft.Extensions.Caching;
```
เรียกใช้ใน Method `ConfigureServices(IServiceCollection services)`

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMemoryCache();
    ...
    ..
    .
}
```
ตัวอย่างการเรียกใช้ 
```csharp
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
```


```json
[
  {
    "Date": "2020-04-08T02:30:08.5662208+07:00",
    "TemperatureC": 10,
    "TemperatureF": 49,
    "Summary": "Bracing with Cache 2020-04-07 02:30"
  },
  {
    "Date": "2020-04-09T02:30:08.5683575+07:00",
    "TemperatureC": 26,
    "TemperatureF": 78,
    "Summary": "Cool with Cache 2020-04-07 02:30"
  },
  {
    "Date": "2020-04-10T02:30:08.5683644+07:00",
    "TemperatureC": 21,
    "TemperatureF": 69,
    "Summary": "Cool with Cache 2020-04-07 02:30"
  },
  {
    "Date": "2020-04-11T02:30:08.5683652+07:00",
    "TemperatureC": 28,
    "TemperatureF": 82,
    "Summary": "Bracing with Cache 2020-04-07 02:30"
  },
  {
    "Date": "2020-04-12T02:30:08.5683658+07:00",
    "TemperatureC": 25,
    "TemperatureF": 76,
    "Summary": "Cool with Cache 2020-04-07 02:30"
  }
]
```

reference : 
[Microsoft ASP.NET Core In-Memory Cache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-3.1#use-imemorycache) |
[Microsoft ASP.NET Core Dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1)

blog : [ใช้ Cache แบบง่าย ๆ บน ASP.NET Core 3.1](https://workwith.coffee/2020/04/%e0%b9%83%e0%b8%8a%e0%b9%89-cache-%e0%b9%81%e0%b8%9a%e0%b8%9a%e0%b8%87%e0%b9%88%e0%b8%b2%e0%b8%a2-%e0%b9%86-%e0%b8%9a%e0%b8%99-asp-net-core/)