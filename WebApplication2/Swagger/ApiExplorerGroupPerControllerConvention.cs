using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;

namespace WebApplication2.Swagger
{
	public class ApiExplorerGroupPerControllerConvention : IControllerModelConvention
	{
		public void Apply(ControllerModel controller)
		{
			string group = controller.ControllerName;
			if (controller.Properties.TryGetValue(typeof(ApiVersionModel), out object objectRawVersion) && objectRawVersion is ApiVersionModel version)
			{
				group = $"{version.DeclaredApiVersions.First()}.{controller.ControllerName}";
			}
			controller.ApiExplorer.GroupName = group;
        }
	}
}
