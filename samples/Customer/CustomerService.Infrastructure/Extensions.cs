using System;
using AppContracts.RestApi;
using CustomerService.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N8T.Infrastructure;
using N8T.Infrastructure.Bus;
using N8T.Infrastructure.EfCore;
using N8T.Infrastructure.Logging;
using N8T.Infrastructure.ServiceInvocation.Dapr;
using N8T.Infrastructure.Swagger;
using N8T.Infrastructure.TransactionalOutbox;
using N8T.Infrastructure.Validator;
using AppCoreAnchor = CustomerService.AppCore.Anchor;

namespace CustomerService.Infrastructure
{
    public static class Extensions
    {
        private const string CorsName = "api";
        private const string DbName = "postgres";

        public static IServiceCollection AddCoreServices(this IServiceCollection services,
            IConfiguration config,
            IWebHostEnvironment env,
            Type apiType)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsName, policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddHttpContextAccessor();
            services.AddCustomMediatR(new[] { typeof(AppCoreAnchor) });
            services.AddCustomValidators(new[] { typeof(AppCoreAnchor) });
            services.AddDaprClient();
            services.AddControllers().AddMessageBroker(config);
            services.AddTransactionalOutbox(config);
            services.AddSwagger(apiType);

            services.AddPostgresDbContext<MainDbContext>(
                config.GetConnectionString(DbName),
                dbOptionsBuilder => dbOptionsBuilder.UseModel(MainDbContextModel.Instance),
                svc => svc.AddRepository(typeof(Repository<>)));

            services.AddRestClient(typeof(ICountryApi),
                config.GetValue("Services:SettingApp:Host", "localhost"),
                config.GetValue("Services:SettingApp:Port", 5005));

            return services;
        }

        public static IApplicationBuilder UseCoreApplication(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.CreateLoggerConfiguration(false);
            app.UseCors(CorsName);
            app.UseRouting();
            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapDefaultControllerRoute();
            });

            IApiVersionDescriptionProvider? provider = app.Services.GetService<IApiVersionDescriptionProvider>();
            return app.UseSwagger(provider);
        }
    }
}
