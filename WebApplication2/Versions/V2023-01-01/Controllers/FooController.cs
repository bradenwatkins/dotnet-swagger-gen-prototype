using System.Collections.Generic;
using BillingRP.Swagger;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Versions.V2023_01_01.Controllers
{
	[Route("[controller]")]
	public class FooController : ControllerBase
	{
		private readonly Dictionary<string, FooModel> _foos;

		public FooController()
		{
			_foos = new Dictionary<string, FooModel>
			{
				{ "1", new FooModel { Id = "1", Name = "Microsoft" } },
				{ "2", new FooModel { Id = "2", Name = "BillingRP" } }
			};
		}

		[HttpGet("{id}")]
		[ArmSwaggerResource(
			noun: "Foos",
			verb: "Get",
			Description = "Get a Foo.",
			Examples = new[] { typeof(FooControllerExamples) })]
		public ActionResult<FooModel> GetFoo(string id)
		{
			if (_foos.TryGetValue(id, out FooModel model))
			{
				return Ok(model);
			}

			return NotFound();
		}
	}
}
