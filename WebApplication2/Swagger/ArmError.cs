using System.Collections.Generic;

namespace BillingRP.Swagger
{
	public class ArmError
	{
		public string Code { get; set; }

		public IEnumerable<ArmErrorDetail> Details { get; set; }

		public string Message { get; set; }

		public string Target { get; set; }
	}
}
