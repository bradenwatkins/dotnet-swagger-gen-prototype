using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BillingRP.Swagger
{
    public static class ArmSwaggerExtensions
    {
        public static IServiceCollection AddArmSwagger(this IServiceCollection services)
        {
			services.AddSwaggerGen();
			services.AddSwaggerGenNewtonsoftSupport();
			services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

			return services;
        }

        public static IApplicationBuilder UseArmSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider versionProvider)
        {
			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger(options =>
			{
				options.SerializeAsV2 = true;
			});

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			var endpointTemplate = "/swagger/{0}/swagger.json";
			app.UseSwaggerUI(options =>
			{
				// Build a swagger endpoint for each discovered API version.
				foreach (var description in versionProvider.ApiVersionDescriptions)
				{
					options.SwaggerEndpoint(string.Format(endpointTemplate, description.GroupName), description.GroupName);
				}

				options.RoutePrefix = string.Empty;
			});

			/* 
			TODO: Generate swagger files at build/runtime to be included with BillingRP pull requests
				- Possible alternative solution: https://medium.com/@woeterman_94/how-to-generate-a-swagger-json-file-on-build-in-net-core-fa74eec3df1
			var webHost = WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
			var sw = webHost.Services.GetRequiredService<ISwaggerProvider>();
			foreach (var description in versionProvider.ApiVersionDescriptions)
			{
				var doc = sw.GetSwagger(string.Format(endpointTemplate, description.GroupName));
				using (var streamWriter = new StringWriter())
				{
					var writer = new OpenApiJsonWriter(streamWriter);
					doc.SerializeAsV2(writer);
					File.WriteAllText("../documentation/swaggerV2.json", writer.ToString());
				}
			}
			*/

			return app;
        }
    }
}
