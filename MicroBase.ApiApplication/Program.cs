using AutoMapper;
using MicroBase.FileStorage.Extensions;
using MicroBase.ApiApplication.Extensions;
using MicroBase.BaseMvc.Middlewares;
using MicroBase.Entity;
using MicroBase.NoDependencyService;
using MicroBase.RabbitMQ;
using MicroBase.RedisProvider;
using MicroBase.Service.AutoMap;
using MicroBase.Service.Extensions;
using MicroBase.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using MicroBase.BaseApi.ServiceModule;
using MicroBase.BaseMvc.Filters;
using MicroBase.BaseMvc;
using MicroBase.Share.Models;
using Microsoft.AspNetCore.Mvc;
using MicroBase.Share.Constants;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerHeaderFilter>();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Collection", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
        },
        new string[] { }
    }});
});

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);

builder.WebHost.UseUrls("http://0.0.0.0:6666");

#region Register Services

OracleModuleRegister.AddMicroDbContext(builder.Services, Configuration);
BaseApiServiceModule.ModuleRegister(builder.Services, Configuration);
NoDependencyServiceModule.ModuleRegister(builder.Services, Configuration);
IdentityServiceModule.ModuleRegister(builder.Services, Configuration);
TelegramServiceModule.ModuleRegister(builder.Services, Configuration);
RedisServiceModule.ModuleRegister(builder.Services, Configuration);
RabbitMqServiceModule.ModuleRegister(builder.Services, Configuration);

ApiServiceModule.ModuleRegister(builder.Services, Configuration);
FileUploadServiceModule.ModuleRegister(builder.Services, Configuration);

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new IdMappingProfile());
    mc.AddProfile(new BaseSolutionMappingProfile());
});

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// IHttpClientFactory can be registered by calling AddHttpClient:
builder.Services.AddHttpClient();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#endregion

builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var modelState = actionContext.ModelState.Values;
        return new BadRequestObjectResult(new BaseResponse<object>
        {
            Success = false,
            Message = CommonMessage.MODEL_STATE_INVALID,
            Errors = ModelStateService.GetModelStateErros(actionContext.ModelState)
        });
    };
}).AddJsonOptions(options => { });

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

//app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Run();