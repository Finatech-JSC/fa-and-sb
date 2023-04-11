using MicroBase.Service.Foundations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.BaseApi.ServiceModule
{
    public static class BaseApiServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddTransient(typeof(ICrudAppService<,,,>), typeof(CrudAppService<,,,>));
        }
    }
}