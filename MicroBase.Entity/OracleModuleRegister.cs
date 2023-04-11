using MicroBase.Entity.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroBase.Entity
{
    public static class OracleModuleRegister
    {
        public static void AddMicroDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnStr = configuration.GetConnectionString("MainConn");
            services.AddDbContext<MicroDbContext>(optionsBuilder => optionsBuilder.UseOracle(sqlConnStr));
            
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
        }
    }
}