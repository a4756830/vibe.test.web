using Microsoft.AspNetCore.Mvc;
using Vibe.Test.Servcie.Enums;
using Vibe.Test.Servcie.ViewModel;

namespace Vibe.Test.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<APIResult> Get()
        {
            var result = new Result<List<WeatherForecast>>();
            try
            {
                var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToList();

                result.Data = forecasts;
                result.Count = forecasts.Count;
                result.Success("取得天氣預報成功");
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Fail($"取得天氣預報失敗: {ex.Message}", ApiReturnCode.General_Error);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
