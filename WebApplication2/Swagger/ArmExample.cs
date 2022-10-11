using System.Collections.Generic;

namespace BillingRP.Swagger
{
    public abstract class ArmExample
    {
        public abstract Dictionary<string, object> Parameters { get; }

        public abstract Dictionary<int, ArmExampleResponse> Responses { get; }
    }
}
