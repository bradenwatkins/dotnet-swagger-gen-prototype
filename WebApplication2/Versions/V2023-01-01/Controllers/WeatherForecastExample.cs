using System;
using System.Collections.Generic;
using BillingRP.Swagger;
using Microsoft.AspNetCore.Http;

namespace WebApplication2.Versions.V2023_01_01.Controllers
{
	public class WeatherForecastExample : ArmExample
	{
		public override Dictionary<string, object> Parameters => new Dictionary<string, object>();

		public override Dictionary<int, ArmExampleResponse> Responses => new Dictionary<int, ArmExampleResponse>
		{
			{
				StatusCodes.Status200OK,
				new ArmExampleResponse
				{
					Body = new List<WeatherForecast>
					{
						new WeatherForecast
						{
							 Date = DateTime.UtcNow,
							 Summary = "Summary",
							 TemperatureCelcius = 42
						}
					},
					Headers = new Dictionary<string, string>
					{
						{ "x-ms-foo", "bar" }
					}
				}
			}
		};
	}
}
