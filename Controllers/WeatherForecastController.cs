using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        
        private readonly IWeatherForecastService _service;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("generate")]
        public ActionResult<IEnumerable<WeatherForecast>> Generate([FromQuery] int count,
            [FromBody] TemperatureRequest request)
        {
            if (count < 0 || request.Max < request.Min)
            {
                return BadRequest();
            }
            var result = _service.Get(count, request.Min, request.Max);
            return Ok(result);
        }
        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var results = _service.Get();
        //    return results;
        //}

        //[HttpGet("currentDay/{max}")]
        ////[Route("currentDay")]
        //public IEnumerable<WeatherForecast> Get2([FromQuery]int take, [FromRoute]int max)
        //{
        //    var results = _service.Get();
        //    return results;
        //}
        //[HttpPost]
        ////Dzięki actionresult możemy zdefiniowac typ zwracanego statusu
        //public ActionResult<string> Hello([FromBody] string name)
        //{
        //    //HttpContext.Response.StatusCode = 401;
        //    //return $"Hello {name}";

        //    //return StatusCode(401,$"Hello {name}");

        //    return NotFound($"Hello {name}");
    }
}
