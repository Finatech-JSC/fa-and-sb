// See https://aka.ms/new-console-template for more information
using MicroBase.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Start Migrator Service");

// Set up configuration sources.
IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// implementatinon of 'CreateHostBuilder' static method and create host object
IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            OracleModuleRegister.AddMicroDbContext(services, Configuration);
        });
}

// create hosting object and DI layer
using IHost host = CreateHostBuilder(args).Build();

// create a service scope
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    Console.WriteLine("Start Migration Process");

    var context = services.GetRequiredService<MicroDbContext>();
    context.Database.Migrate();

    Console.WriteLine("End Migration Process");
}