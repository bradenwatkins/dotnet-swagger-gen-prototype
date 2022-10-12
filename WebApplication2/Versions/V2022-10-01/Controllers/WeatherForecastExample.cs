using System;
using System.Collections.Generic;
using BillingRP.Swagger;
using Microsoft.AspNetCore.Http;

namespace WebApplication2.Versions.V2022_10_01.Controllers
{
	public class WeatherForecastExample : ArmExample
	{
		public override Dictionary<string, object> Parameters => new Dictionary<string, object>
		{
			{ "api-version", "2022-10-01" }
		};

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
							 Date = new DateTime(2022, 10, 12, 10, 03, 00),
							 Summary = "Summary",
							 TemperatureC = 42
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
