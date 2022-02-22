using System;
using System.Security.Claims;
using System.Text;
using AppContracts.RestApi;
using IdentityService.AppCore.Core.Models;
using IdentityService.AppCore.Enums;
using IdentityService.AppCore.Interfaces;
using IdentityService.Infrastructure.Cache;
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
using N8T.Infrastructure.Cache;
using N8T.Infrastructure.EfCore;
using N8T.Infrastructure.ServiceInvocation.Dapr;
using N8T.Infrastructure.Swagger;
using N8T.Infrastructure.TransactionalOutbox;
using N8T.Infrastructure.Validator;
using AppCoreAnchor = IdentityService.AppCore.Anchor;
using InfrastructureAnchor = IdentityService.Infrastructure.Anchor;

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


            services.AddCustomAuth(config,
                new[] { typeof(AppCoreAnchor), typeof(InfrastructureAnchor) },
                bearerOptions =>
                {
                    bearerOptions.RequireHttpsMetadata = false;
                    bearerOptions.SaveToken = true;
                    bearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config.GetValue<string>("Security:Tokens:Key")))
                    };
                },
                authorizationOptions =>
                {
                    authorizationOptions.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                    {
                        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim(ClaimTypes.Name);
                        policy.RequireClaim(ClaimTypes.NameIdentifier);
                        policy.RequireClaim(ClaimTypes.Role);
                        policy.RequireClaim("scope");
                    });

                    authorizationOptions.AddPolicy("UserRole", policy =>
                    {
                        policy.RequireRole(RoleEnum.User);
                    });

                    authorizationOptions.AddPolicy("AdminRole", policy =>
                    {
                        policy.RequireRole(RoleEnum.Admin);
                    });

                    authorizationOptions.AddPolicy("ApiCaller", policy =>
                    {
                        policy.RequireClaim("scope", "api");
                    });
                });

            services.AddAuthorization();

            services.AddSingleton(typeof(ICacheService<>), typeof(RedisCacheService<>));

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


            services.AddCustomMediatR(new[] { typeof(AppCoreAnchor) });
            services.AddCustomValidators(new[] { typeof(AppCoreAnchor) });

            services.AddScoped(typeof(ISecurityContextAccessor), typeof(SecurityContextAccessor));
            services.AddTransient(typeof(IIdentityUserService), typeof(IdentityUserService));

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
            this IApplicationBuilder app,
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
                endpoints.MapControllers()
                    .RequireAuthorization(JwtBearerDefaults.AuthenticationScheme);
            });

            IApiVersionDescriptionProvider? provider =
                app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            return app.UseSwagger(provider);
        }
    }
}
