using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BillingRP.Swagger
{
    // R2018 XmsEnumValidation - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r2018-xmsenumvalidation
    public class ArmSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            ApplyArmResourceRules(schema, context, typeof(ArmResource<>));
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

        private static void ApplyArmResourceRules(OpenApiSchema schema, SchemaFilterContext context, Type resourceType)
        {
            var isArmResourceType = context.Type.IsGenericType && context.Type.GetGenericTypeDefinition().IsAssignableFrom(resourceType);
            if (!isArmResourceType)
            {
                return;
            }

            if (!context.Type.GenericTypeArguments.Any())
            {
                schema.Properties.Remove("properties");
                return;
            }

            if (!context.SchemaRepository.TryLookupByType(resourceType, out var armResourceSchema))
            {
                armResourceSchema = context.SchemaGenerator.GenerateSchema(resourceType, context.SchemaRepository);
            }

            var propertiesReference = schema.Properties["properties"].Reference;
            var propertiesId = propertiesReference.Id;
            var propertiesSchema = context.SchemaRepository.Schemas.GetValueOrDefault(propertiesId);

            var description = propertiesSchema.Description ?? schema.Description;
            description = string.IsNullOrWhiteSpace(description) ? "Resource" : description;
            schema.Properties.Clear();
            schema.Description = description;
            schema.AllOf.Add(armResourceSchema);
            schema.Extensions.Add("x-ms-azure-resource", new OpenApiBoolean(true));
            schema.Properties.Add("properties", new OpenApiSchema
            {
                Description = $"The properties of a(n) {propertiesId[..^"properties".Length]}",
                Extensions = new Dictionary<string, IOpenApiExtension> { { "x-ms-client-flatten", new OpenApiBoolean(true) } }
            });

            if (propertiesId != "TPropertiesProperties" && propertiesId != "TPatchPropertiesProperties")
            {
                schema.Properties["properties"].AllOf = new[] { new OpenApiSchema { Reference = propertiesReference } };
            }

            return;
        }
    }
}
