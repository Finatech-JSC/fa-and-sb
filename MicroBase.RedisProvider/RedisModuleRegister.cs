using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.RedisProvider
{
    public static class RedisModuleRegister
    {
        public static void AddRedisCache(this IServiceCollection services, IConfiguration Configuration)
        {
            RedisStaticModel.RedisConnection = Configuration.GetValue<string>("Redis:Connection");
            services.AddSingleton<IRedisStogare, RedisStogare>();
        }
    }
}