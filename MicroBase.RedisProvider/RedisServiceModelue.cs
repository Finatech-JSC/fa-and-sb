using MicroBase.Share;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.RedisProvider
{
    public static class RedisServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            RedisStaticModel.RedisConnection = Configuration.GetValue<string>("Redis:Connection");
            services.AddTransient<IRedisStogare, RedisStogare>();
        }
    }
}