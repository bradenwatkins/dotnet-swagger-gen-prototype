using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BillingRP.Swagger
{
    // R1001 OperationIdNounVerb - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r1001
    // R2054 SecurityDefinitionsStructure - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r2054-securitydefinitionsstructure
    // R2055 OneUnderscoreInOperationId - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r2055-oneunderscoreinoperationid
    // R4004 OperationIdRequired - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r4004
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var deprecatedVersions = new List<string>
            {
                "2017-02-27-preview",
                "2017-04-24",
                "2017-04-24-preview",
                "2018-03-01-preview",
                "2018-11-01-preview",
                "2020-10-01"
            };

            // Workaround for when model class name isn't unique among all the model classes in an ApiVersion, names are much longer, however.
            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1607
            // The alternative is annotate Model classes to fix their class names to not be duplicate: Swashbuckle.AspNetCore.Annotations
            options.CustomSchemaIds(type =>
            {
                var fullName = type.ToString();
                if (type.GenericTypeArguments.Any())
                {
                    Type generic = type;
                    do
                    {
                        generic = generic.GenericTypeArguments.First();
                    }
                    while (generic.ToString().IndexOf("ArmResource", StringComparison.OrdinalIgnoreCase) != -1);

                    return fullName.IndexOf("ArmResourceContainer", StringComparison.OrdinalIgnoreCase) != -1
                        ? $"{generic.Name}ListResult"
                            : fullName.IndexOf("EntityPage", StringComparison.OrdinalIgnoreCase) != -1
                                ? $"{generic.Name}PageResult"
                                : $"{generic.Name}Resource";
                }

                return type.Name;
            });

            // Create docs for each ApiVersion.
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var isDeprecated = deprecatedVersions.Contains(description.GroupName);
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $"BillingRP API {description.GroupName}{(isDeprecated ? " (Deprecated)" : string.Empty)}",
                    Version = description.GroupName,
                    Description = "BillingRP API",
                    Contact = new OpenApiContact
                    {
                        Name = "Billing Platform",
                        Email = "cabbpt@microsoft.com",
                        Url = new Uri("https://docs.microsoft.com/en-us/rest/api/billing/")
                    }
                });
            }

            options.DocInclusionPredicate((versionName, apiDescription) =>
            {
                if (apiDescription.RelativePath.IndexOf("/operationResults/", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return false;
                }

                var versionModel = apiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Implicit | ApiVersionMapping.Explicit);

                if (versionModel == null)
                {
                    return true;
                }

                if (versionModel.DeclaredApiVersions.Any())
                {
                    return versionModel.DeclaredApiVersions.Any(v => v.ToString() == versionName);
                }

                return versionModel.ImplementedApiVersions.Any(v => v.ToString() == versionName);
            });

            options.DocumentFilter<ArmDocumentFilter>();
            options.SchemaFilter<ArmSchemaFilter>();
            options.OperationFilter<ArmOperationFilter>();
            // options.OperationFilter<Versions.V2023_01_01.Swagger.BillingRPOperationFilter>();

            options.AddSecurityDefinition("azure_auth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Azure Active Directory OAuth2 Flow.",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        Scopes = new Dictionary<string, string>
                        {
                            { "user_impersonation", "impersonate your user account" },
                        }
                    }
                }
            });
        }
    }
}
