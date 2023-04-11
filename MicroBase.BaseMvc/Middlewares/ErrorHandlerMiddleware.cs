using MicroBase.Service.Localizations;
using MicroBase.Share.Models;
using MicroBase.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using MicroBase.Share.Constants;

namespace MicroBase.BaseMvc.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate _next)
        {
            this._next = _next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var services = httpContext.RequestServices;

                var actions = new List<string>();
                var routeDatas = httpContext.GetRouteData().Values;
                foreach (var item in routeDatas)
                {
                    actions.Add(item.Value.ToString());
                }

                actions.Reverse();

                var exceptionMonitorService = (IExceptionMonitorService)services.GetService(typeof(IExceptionMonitorService));
                await exceptionMonitorService.SendExceptionNotiAsync(string.Join(" > ", actions.ToArray()), ex);

                var localizationService = (ILocalizationService)services.GetService(typeof(ILocalizationService));

                var response = httpContext.Response;
                response.ContentType = "application/json";
                var res = new BaseResponse<object>
                {
                    Success = false,
                    Message = localizationService.GetLocalizationString(CommonMessage.UN_DETECTED_ERROR, null, null, string.Empty),
                    MessageCode = nameof(CommonMessage.UN_DETECTED_ERROR)
                };

                var result = JsonConvert.SerializeObject(res);
                await response.WriteAsync(result);
            }
        }
    }
}
