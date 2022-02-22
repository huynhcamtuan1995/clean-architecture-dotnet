using System;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace N8T.Infrastructure.Auth
{
    public static class Extensions
    {
        public static IServiceCollection AddCustomAuth(this IServiceCollection services,
            IConfiguration config,
            Type[] types,
            Action<JwtBearerOptions> configureJwtBearer = null,
            Action<AuthorizationOptions> configureAuthorizations = null,
            Action<IServiceCollection> configureMoreActions = null)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    config.Bind("Auth", options);
                    configureJwtBearer?.Invoke(options);
                });

            services.AddAuthorization(options =>
            {
                configureAuthorizations?.Invoke(options);
            });

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>));

            services.Scan(s => s
                .FromAssembliesOf(types)
                .AddClasses(c => c
                    .AssignableTo<IAuthorizationHandler>()).As<IAuthorizationHandler>()
                .AddClasses(c => c
                    .AssignableTo<IAuthorizationRequirement>()).As<IAuthorizationRequirement>());

            configureMoreActions?.Invoke(services);

            return services;
        }
    }
}
