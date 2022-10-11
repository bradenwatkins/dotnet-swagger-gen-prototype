using System;

namespace BillingRP.Swagger
{
    public class ArmSwaggerResourceAttribute
    {
        public string Operation { get; set; }

        public Type[] Examples { get; set; }

        public ArmSwaggerResourceAttribute(string noun, string verb)
        {
			Operation = $"{noun}_{verb}";
        }
    }
}
