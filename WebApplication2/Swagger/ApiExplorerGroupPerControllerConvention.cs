using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApplication2.Swagger
{
	public class ApiExplorerGroupPerControllerConvention : IControllerModelConvention
	{
		public void Apply(ControllerModel controller)
		{
            controller.ApiExplorer.GroupName = controller.GetGroupName();
        }
	}
}
