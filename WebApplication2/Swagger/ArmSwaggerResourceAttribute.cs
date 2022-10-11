using System;

namespace BillingRP.Swagger
{
    public class ArmSwaggerResourceAttribute : Attribute
	{
        public string Operation { get; set; }

        public string Description { get; set; }

        public Type[] Examples { get; set; }

        public ArmSwaggerResourceAttribute(string noun, string verb)
        {
			Operation = $"{noun}_{verb}";
        }
    }
}
