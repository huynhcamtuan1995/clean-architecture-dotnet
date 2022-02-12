using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace N8T.Infrastructure.Swagger
{
    public static class Extentions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, Type anchor)
        {
            services.AddApiVersioning(
                options =>
                {
                    options.ReportApiVersions = true;
                });
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    options.SubstituteApiVersionInUrl = true;
                });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                {
                    options.OperationFilter<SwaggerDefaultValues>();

                    string xmlFile = XmlCommentsFilePath(anchor);
                    if (File.Exists(xmlFile))
                    {
                        options.IncludeXmlComments(xmlFile);
                    }
                });

            return services;

            static string XmlCommentsFilePath(Type anchor)
            {
                string basePath = PlatformServices.Default.Application.ApplicationBasePath;
                string fileName = anchor.GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });

            return app;
        }
    }
}
