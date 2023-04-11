using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace MicroBase.BaseMvc.Attributes
{
    public static class AttributeExtensions
    {
        public static IEnumerable<T> GetCustomAttributes<T>(this ActionDescriptor actionDescriptor) where T : Attribute
        {
            var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                return controllerActionDescriptor.MethodInfo.GetCustomAttributes<T>();
            }

            return Enumerable.Empty<T>();
        }
    }
}
