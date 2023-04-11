using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityUser = MicroBase.Entity.Accounts.IdentityUser;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MicroBase.Entity;
using MicroBase.Service.Accounts;
using MicroBase.Entity.Accounts;

namespace MicroBase.Service.Extensions
{
    public static class IdentityServiceModule
    {
        public static void ModuleRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddIdentity<IdentityUser, IdentityUserRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // User settings
                options.User.RequireUniqueEmail = true;

                // Allow all characters
                options.User.AllowedUserNameCharacters = null;

                // Require a confirmed email
                options.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<MicroDbContext>()
            .AddDefaultTokenProviders();

            services.TryAddSingleton<UserManager<IdentityUser>, UserManager<IdentityUser>>();
            services.TryAddScoped<SignInManager<IdentityUser>, SignInManager<IdentityUser>>();

            services.AddTransient<IAccountLoginService, AccountLoginService>();
            services.AddTransient<IGoogleAuthenticatorService, GoogleAuthenticatorService>();
            services.AddTransient<IAccountManageService, AccountManageService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IPermissionManageService, PermissionManageService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IRoleGroupService, RoleGroupService>();
            services.AddTransient<IRolePermissionService, RolePermissionService>();
            services.AddTransient<ISystemMenuService, SystemMenuService>();
        }
    }
}