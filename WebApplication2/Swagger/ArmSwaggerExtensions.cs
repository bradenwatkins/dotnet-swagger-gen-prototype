using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication2.Swagger;

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

        public static IApplicationBuilder UseArmSwagger(this IApplicationBuilder app, IApiDescriptionGroupCollectionProvider descriptionProvider)
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
				foreach (var groupName in descriptionProvider.ApiDescriptionGroups.GetGroupNames())
				{
					options.SwaggerEndpoint(string.Format(endpointTemplate, groupName), groupName);
				}
			});

			return app;
        }
    }
}
