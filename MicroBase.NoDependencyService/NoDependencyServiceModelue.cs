using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.NoDependencyService
{
    public static class NoDependencyServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRandomService, RandomService>();
            services.AddTransient<IDataGridService, DataGridService>();
        }
    }
}