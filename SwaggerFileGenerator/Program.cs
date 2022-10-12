using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using WebApplication2;

namespace SwaggerFileGenerator
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// TODO: Commandline arg for by versions versus by controllers.
			string basePath = Path.Combine(AppContext.BaseDirectory, "docs");
			Directory.CreateDirectory(basePath);

			using var host = new HostBuilder()
				.ConfigureWebHost(builder =>
				{
					builder
						.UseTestServer()
						.ConfigureServices(services =>
						{
							// setup fakes if needed.
							// Or setup an overrideing ConfigureSwaggerOptions to influence swagger gen, ex by versions versus by controller.
						})
					// Setup a different environment if that's how we want to influence swagger gen, ex by versions versus by controller.
					// .UseEnvironment("") 
						.UseStartup<Startup>();
				})
			.Build();

			var serviceProvider = host.Services;
			var swaggerProvider = serviceProvider.GetRequiredService<ISwaggerProvider>();
			var apiProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

			// If by version!
			string versionPath = Path.Combine(basePath, "versions");
			Directory.CreateDirectory(versionPath);
			foreach (var version in apiProvider.ApiVersionDescriptions)
			{
				var swaggerPath = Path.Combine(versionPath, $"{version.GroupName}.json");
				var swagger = swaggerProvider.GetSwagger(version.GroupName);

				// Option 1:
				// using StreamWriter streamWriter = new StreamWriter(swaggerPath);
				// var swaggerWriter = new OpenApiJsonWriter(streamWriter);
				// swagger.SerializeAsV3(swaggerWriter);

				// Option 2:
				using FileStream swaggerFile = File.Create(swaggerPath);
				swagger.SerializeAsJson(swaggerFile, OpenApiSpecVersion.OpenApi3_0);
			}

			// If by controller!
			string controllersPath = Path.Combine(basePath, "controllers");
			Directory.CreateDirectory(controllersPath);
			var controllers = typeof(Startup).Assembly.GetTypes().Where(t => t.BaseType == typeof(ControllerBase));
			foreach (var controller in controllers)
			{
				// TODO: We don't actually have support for swagger gen by controller instead of api version.
				// TODO: Probably parse namespace to get Api-version?
				Console.WriteLine(controller.Name);
			}
		}
	}
}
