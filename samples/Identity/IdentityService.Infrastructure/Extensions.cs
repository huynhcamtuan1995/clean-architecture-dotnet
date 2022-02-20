using System;
using System.Security.Claims;
using System.Text;
using AppContracts.RestApi;
using IdentityService.AppCore.Core.Models;
using IdentityService.AppCore.Interfaces;
using IdentityService.Infrastructure.Data;
using IdentityService.Infrastructure.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using N8T.Infrastructure;
using N8T.Infrastructure.Auth;
using N8T.Infrastructure.Bus;
using N8T.Infrastructure.EfCore;
using N8T.Infrastructure.ServiceInvocation.Dapr;
using N8T.Infrastructure.Swagger;
using N8T.Infrastructure.TransactionalOutbox;
using N8T.Infrastructure.Validator;
using AppCoreAnchor = IdentityService.AppCore.Anchor;

namespace IdentityService.Infrastructure
{
    public static class Extensions
    {
        private const string CorsApiName = "api";
        private const string DbName = "postgres";

        public static IServiceCollection AddCoreServices(
            this IServiceCollection services,
            IConfiguration config,
            IWebHostEnvironment env,
            Type apiType)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsApiName, policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddHttpContextAccessor();

            services.AddCustomAuth<AppCoreAnchor>(config,
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config.GetValue<string>("Security:Tokens:Key")))
                    };
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Name);
                });

                options.AddPolicy("UserRole", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role);
                });

                options.AddPolicy("ApiCaller", policy =>
                {
                    policy.RequireClaim("scope");
                });

                options.AddPolicy("RequireInteractiveUser", policy =>
                {
                    policy.RequireClaim("sub");
                });
            });

            services.AddPostgresDbContext<MainDbContext>(
                config.GetConnectionString(DbName));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                      .AddUserManager<UserManager<ApplicationUser>>()
                      .AddSignInManager<SignInManager<ApplicationUser>>()
                      .AddDefaultTokenProviders()
                      .AddEntityFrameworkStores<MainDbContext>();

            services.Configure<IdentityOptions>(
                options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                    // Identity : Default password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;
                });

            services.AddTransient(typeof(IUserIdentityService), typeof(UserIdentityService));

            services.AddCustomMediatR(new[] { typeof(AppCoreAnchor) });
            services.AddCustomValidators(new[] { typeof(AppCoreAnchor) });

            //services.AddDaprClient();
            services.AddControllers().AddMessageBroker(config);
            services.AddTransactionalOutbox(config);
            services.AddSwagger(apiType);
            services.AddRestClient(typeof(ICustomerApi),
                config.GetValue("Services:CustomerApp:Host", "localhost"),
                config.GetValue("Services:CustomerApp:Port", 5004));

            return services;
        }

        public static IApplicationBuilder UseCoreApplication(
            this WebApplication app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(CorsApiName);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });

            IApiVersionDescriptionProvider? provider =
                app.Services.GetService<IApiVersionDescriptionProvider>();
            return app.UseSwagger(provider);
        }
    }
}
