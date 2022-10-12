using System.Collections.Generic;
using BillingRP.Swagger;
using Microsoft.AspNetCore.Http;

namespace WebApplication2.Versions.V2023_01_01.Controllers
{
	public class FooControllerExamples : ArmExample
	{
		public override Dictionary<string, object> Parameters => new Dictionary<string, object>
		{
			{ "api-version", "2023-01-01" },
			{ "id", "{id}" }
		};

		public override Dictionary<int, ArmExampleResponse> Responses => new Dictionary<int, ArmExampleResponse>
		{
			{
				StatusCodes.Status200OK,
				new ArmExampleResponse
				{
					Body = new FooModel
					{
						Id = "1",
						Name = "Example"
					}
				}
			}
		};
	}
}
