using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BillingRP.Swagger
{
    public class ArmOperationFilter : IOperationFilter
    {
        private static readonly string[] ArmErrorStatusCodes = new[]
        {
            "default",
            StatusCodes.Status400BadRequest.ToString(),
            StatusCodes.Status401Unauthorized.ToString(),
            StatusCodes.Status403Forbidden.ToString(),
            StatusCodes.Status404NotFound.ToString(),
            StatusCodes.Status405MethodNotAllowed.ToString(),
            StatusCodes.Status500InternalServerError.ToString(),
            StatusCodes.Status501NotImplemented.ToString()
        };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            ApplyRemoveConsumes(operation, context);
            ApplyArmResponses(operation, context);
            ApplyArmResourceAttribute(operation, context);

            operation.ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("https://docs.microsoft.com/en-us/rest/api/billing/")
            };
        }

        private static void ApplyRemoveConsumes(OpenApiOperation operation, OperationFilterContext context)
        {
            // Remove "consumes" from individual controllers
            var keysToRemove = operation.RequestBody?.Content.Keys.Where(key => key != "application/json");
            if (keysToRemove != null)
            {
                foreach (var key in keysToRemove)
                {
                    operation.RequestBody.Content.Remove(key);
                }
            }
        }

        // R4010 RequiredDefaultResponse - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r4010-requireddefaultresponse
        // R4007 DefaultErrorResponseSchema - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r4007-defaulterrorresponseschema
        private static void ApplyArmResponses(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var (key, _) in operation.Responses)
            {
                // Ensure that all error codes return type ArmError
                if (ArmErrorStatusCodes.Contains(key))
                {
                    operation.Responses[key].Content = CreateArmErrorResponse(context).Content;
                }

                // Remove all produces except application/json
                var contentKeysToRemove = operation.Responses[key].Content.Keys.Where(key => key != "application/json");
                foreach (var contentKey in contentKeysToRemove)
                {
                    operation.Responses[key].Content.Remove(contentKey);
                }

                // Ensure long running operations have the long-running flag
                if (key == StatusCodes.Status202Accepted.ToString())
                {
                    operation.Responses[key].AddExtension("x-ms-long-running-operation", new OpenApiBoolean(true));
                }
            }

            if (!operation.Responses.ContainsKey("default"))
            {
                operation.Responses.Add("default", CreateArmErrorResponse(context));
            }
        }

        private static void ApplyArmResourceAttribute(OpenApiOperation operation, OperationFilterContext context)
        {
            var armResourceAttribute = context.ApiDescription.CustomAttributes().OfType<ArmSwaggerResourceAttribute>().FirstOrDefault();
            if (armResourceAttribute != null)
            {
                operation.OperationId = armResourceAttribute.Operation;
                if (armResourceAttribute.Examples != null)
                {
                    var examples = new Dictionary<string, string>();
                    foreach (var example in armResourceAttribute.Examples)
                    {
                        examples.Add(example.Name, $"#/examples/{example.Name}");
                    }

                    operation.AddExtension("x-ms-examples", OpenApiAnyFactory.CreateFromJson(JsonConvert.SerializeObject(examples)));
                }
            }
        }

        private static OpenApiResponse CreateArmErrorResponse(OperationFilterContext context)
        {
            if (!context.SchemaRepository.TryLookupByType(typeof(ArmError), out var armErrorSchema))
            {
                armErrorSchema = context.SchemaGenerator.GenerateSchema(typeof(ArmError), context.SchemaRepository);
            }

            if (context.SchemaRepository.TryLookupByType(typeof(ProblemDetails), out var problemDetailsSchema))
            {
                context.SchemaRepository.Schemas.Remove(problemDetailsSchema.Reference.Id);
            }

            return new OpenApiResponse
            {
                Description = "default",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json",
                        new OpenApiMediaType
                        {
                            Schema = armErrorSchema
                        }
                    }
                }
            };
        }
    }
}
