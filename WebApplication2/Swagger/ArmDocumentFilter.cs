using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BillingRP.Swagger
{
    public class ArmDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            ApplyExamples(swaggerDoc, context);

            swaggerDoc.Extensions.Add("host", new OpenApiString("management.azure.com"));
            swaggerDoc.Extensions.Add("schemes", new OpenApiArray() { new OpenApiString("https") });
            swaggerDoc.Extensions.Add("consumes", new OpenApiArray() { new OpenApiString("application/json") });
            swaggerDoc.Extensions.Add("produces", new OpenApiArray() { new OpenApiString("application/json") });
        }

        public static void ApplyExamples(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var examples = new Dictionary<string, object>();
            foreach (var apiDescription in context.ApiDescriptions)
            {
                var armResourceAttribute = apiDescription.CustomAttributes().OfType<ArmSwaggerResourceAttribute>().FirstOrDefault();
                if (armResourceAttribute?.Examples != null)
                {
                    foreach (var example in armResourceAttribute.Examples)
                    {
                        examples.Add(example.Name, Activator.CreateInstance(example));
                    }
                }
            }

            var examplesJson = JsonConvert.SerializeObject(examples, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: false, overrideSpecifiedNames: false)
                },
                Converters = new JsonConverter[]
                {
                    new StringEnumConverter(new DefaultNamingStrategy(), allowIntegerValues: false)
                },
                DefaultValueHandling = DefaultValueHandling.Ignore,
            });

            swaggerDoc.Extensions.Add("examples", OpenApiAnyFactory.CreateFromJson(examplesJson));
        }
    }
}
