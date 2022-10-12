using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApplication2.Versions.V2023_01_01.Controllers
{
	[Route("[controller]")]
	public class FullTrustFooController : ControllerBase
	{
		private readonly Dictionary<string, FooModel> _foos;

		public FullTrustFooController()
		{
			_foos = new Dictionary<string, FooModel>
			{
				{ "1", new FooModel { Id = "1", Name = "Microsoft" } },
				{ "2", new FooModel { Id = "2", Name = "BillingRP" } }
			};
		}

		[HttpGet("{id}")]
		[SwaggerOperation(
			Description = "Get a Foo full trust.",
			OperationId = "Get_FoosFullTrust")]
		public ActionResult<FooModel> GetFooFullTrust(string id)
		{
			if (_foos.TryGetValue(id, out FooModel model))
			{
				return Ok(model);
			}

			return NotFound();
		}
	}
}
