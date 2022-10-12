using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using WebApplication2;
using WebApplication2.Swagger;

namespace SwaggerFileGenerator
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string basePath = Path.Combine(AppContext.BaseDirectory, "docs");
			Directory.CreateDirectory(basePath);

			using var host = new HostBuilder()
				.ConfigureWebHost(builder =>
				{
					builder
						.UseTestServer()
						.UseStartup<Startup>();
				})
			.Build();

			var serviceProvider = host.Services;

			CreateSwaggerFiles(serviceProvider, basePath, OpenApiSpecVersion.OpenApi2_0);
			CreateSwaggerFiles(serviceProvider, basePath, OpenApiSpecVersion.OpenApi3_0);
		}

		private static void CreateSwaggerFiles(IServiceProvider serviceProvider, string docsPath, OpenApiSpecVersion version)
		{
			string versionString;
			IEnumerable<string> groupNames;
			var apiProvider = serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

			if (version == OpenApiSpecVersion.OpenApi2_0)
			{
				versionString = "swagger2.0";
				groupNames = apiProvider.ApiDescriptionGroups.GetGroupNames();
			}
			else if (version == OpenApiSpecVersion.OpenApi3_0)
			{
				versionString = "swagger3.0";
				groupNames = apiProvider.ApiDescriptionGroups.GetVersions();
			}
			else
			{
				throw new InvalidOperationException();
			}

			string basePath = Path.Combine(docsPath, versionString);

			string previewPath = Path.Combine(basePath, "preview");
			string stablePath = Path.Combine(basePath, "stable");
			Directory.CreateDirectory(previewPath);
			Directory.CreateDirectory(stablePath);

			var swaggerProvider = serviceProvider.GetRequiredService<ISwaggerProvider>();

			foreach (var groupName in groupNames)
			{
				string finalPath;
				if (groupName.IsFullyQualified())
				{
					var groupVersion = groupName.GetVersion();
					finalPath = groupVersion.Contains("preview")
						? Path.Combine(previewPath, groupVersion)
						: Path.Combine(stablePath, groupVersion);
					Directory.CreateDirectory(finalPath);
				}
				else
				{
					finalPath = groupName.Contains("preview")
						? previewPath
						: stablePath;
				}

				var swaggerPath = Path.Combine(finalPath, $"{groupName.GetControllerName()}.json");
				var swagger = swaggerProvider.GetSwagger(groupName);

				using FileStream swaggerFile = File.Create(swaggerPath);
				swagger.SerializeAsJson(swaggerFile, version);
			}
		}
	}
}
