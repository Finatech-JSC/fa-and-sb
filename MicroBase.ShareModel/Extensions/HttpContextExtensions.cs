using Microsoft.AspNetCore.Http;
using static MicroBase.Share.Constants.Constants;

namespace MicroBase.Share.Extensions
{
    public static class HttpContextExtensions
    {
        private static string GetRequestHeaderValue(this HttpContext httpContext, string headerKey)
        {
            var str = string.Empty;
            if (httpContext != null && httpContext.Request.Headers.ContainsKey(headerKey))
            {
                str = httpContext.Request.Headers[headerKey];
            }

            return str;
        }

        public static string GetRequestCultureCode(this HttpContext httpContext)
        {
            var str = GetRequestHeaderValue(httpContext, HttpHeaderKey.CULTURE_CODE);
            if (!CultureCode.Maps.ContainsKey(str))
            {
                return CultureCode.UnitedStates;
            }

            return str;
        }
    }
}