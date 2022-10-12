using System;

namespace WebApplication2.Versions.V2023_01_01.Controllers
{
	public class WeatherForecast
	{
		public DateTime Date { get; set; }

		public int TemperatureCelcius { get; set; }

		public int TemperatureFahrenheit => 32 + (int)(TemperatureCelcius / 0.5556);

		public string Summary { get; set; }
	}
}
