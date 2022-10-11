using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BillingRP.Swagger
{
    // R2018 XmsEnumValidation - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r2018-xmsenumvalidation
    public class ArmSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            ApplyXmsEnum(schema, context);
        }

        private static void ApplyXmsEnum(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                schema.Extensions.Add(
                    "x-ms-enum",
                    new OpenApiObject
                    {
                        ["name"] = new OpenApiString(type.Name),
                        ["modelAsString"] = new OpenApiBoolean(true)
                    });
            }
        }
    }
}
