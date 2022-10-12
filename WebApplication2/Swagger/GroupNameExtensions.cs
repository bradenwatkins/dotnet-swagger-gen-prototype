using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Swagger
{
    public static class GroupNameExtensions
    {
        private static string _template = "{0}.{1}";

        public static IEnumerable<string> GetGroupNames(this ApiDescriptionGroupCollection collection) =>
            collection.Items.SelectMany(group => group.Items.Select(apiDescription => apiDescription.GetGroupName())).Distinct();
            

        public static string GetGroupName(this ApiDescription apiDescription) =>
            apiDescription.ActionDescriptor.GetProperty<ControllerModel>().GetGroupName();

        public static string GetGroupName(this ControllerModel controllerModel)
        {
            var versionName = controllerModel.GetProperty<ApiVersionModel>().DeclaredApiVersions.First().ToString();
            var controllerName = controllerModel.ControllerName;
            return string.Format(_template, versionName, controllerName);
        }
    }
}
