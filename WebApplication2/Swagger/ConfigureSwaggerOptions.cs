using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication2.Swagger;

namespace BillingRP.Swagger
{
    // R1001 OperationIdNounVerb - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r1001
    // R2054 SecurityDefinitionsStructure - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r2054-securitydefinitionsstructure
    // R2055 OneUnderscoreInOperationId - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r2055-oneunderscoreinoperationid
    // R4004 OperationIdRequired - https://github.com/Azure/azure-rest-api-specs/blob/master/documentation/openapi-authoring-automated-guidelines.md#r4004
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly IApiDescriptionGroupCollectionProvider _apiExplorer;

        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider,
            IApiDescriptionGroupCollectionProvider apiExplorer
            )
        {
            _provider = provider;
            _apiExplorer = apiExplorer;
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

            var docNames = _apiExplorer.ApiDescriptionGroups.GetGroupNames();
            // Create docs for each ApiVersion.
            foreach (var groupName in docNames)
            {
                var isDeprecated = deprecatedVersions.Contains(groupName);
                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"BillingRP API",
                    Version = groupName.GetVersion(),
                    Description = "BillingRP API",
                    Contact = new OpenApiContact
                    {
                        Name = "Billing Platform",
                        Email = "cabbpt@microsoft.com",
                        Url = new Uri("https://docs.microsoft.com/en-us/rest/api/billing/")
                    }
                });
            }

            var versions = _apiExplorer.ApiDescriptionGroups.GetVersions();
            // Create docs for each ApiVersion.
            foreach (var versionName in versions)
            {
                var isDeprecated = deprecatedVersions.Contains(versionName);
                options.SwaggerDoc(versionName, new OpenApiInfo
                {
                    Title = $"BillingRP API {versionName}{(isDeprecated ? " (Deprecated)" : string.Empty)}",
                    Version = versionName,
                    Description = "BillingRP API",
                    Contact = new OpenApiContact
                    {
                        Name = "Billing Platform",
                        Email = "cabbpt@microsoft.com",
                        Url = new Uri("https://docs.microsoft.com/en-us/rest/api/billing/")
                    }
                });
            }

            options.DocInclusionPredicate((docName, apiDescription) =>
            {
                if (apiDescription.RelativePath.IndexOf("/operationResults/", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return false;
                }

                if (docName.IsFullyQualified())
                {
                    return docName == apiDescription.GetGroupName();
                }
                else
                {
                    return docName == apiDescription.GetGroupName().GetVersion();
                }

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
