using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using static MicroBase.Share.Constants.Constants;

namespace MicroBase.Share.Swagger
{
    public class HeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            var fields = typeof(HttpHeaderKey).GetFields();
            var names = Array.ConvertAll(fields, field => field.GetValue(null).ToString());

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