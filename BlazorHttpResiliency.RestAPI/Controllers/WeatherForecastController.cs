using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlazorHttpResiliency.RestAPI.Controllers
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
        private static int _callsCount = 0;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public IResult Get(bool forceFail = false)
        {
            _callsCount++;
            if (forceFail && _callsCount<4) //max 3x sesuai yg ada di frontend
            { 
                Console.WriteLine($"{_callsCount}__CONTROLLER__");
                return Results.StatusCode((int)HttpStatusCode.BadGateway); //jush example 
            }
            //throw new Exception($"something, somewhere, went terribly, terribly wrong.");
            _callsCount = 0;
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return Results.Ok(result);
        }
    }
}
