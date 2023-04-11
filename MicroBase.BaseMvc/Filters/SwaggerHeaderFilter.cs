using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MicroBase.BaseMvc.Filters
{
    public class SwaggerHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            //var fields = typeof(Constants.HttpHeaderKey).GetFields();
            //var names = Array.ConvertAll(fields, field => field.GetValue(null).ToString());

            var names = new List<string>
            {
                "X-IP-ADDRESS",
                "X-LOCATION",
                "X-USER-AGENT",
                "X-VIA",
                "X-CULTURE-CODE",
                "X-TIMESTAMP"
            };

            foreach (var item in names)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = item,
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "String"
                    }
                });
            }
        }
    }
}