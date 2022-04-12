using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Elasticsearch;

namespace N8T.Infrastructure.Logging
{
    public static class Extensions
    {
        public static void CreateLoggerConfiguration(
            this WebApplication builder,
            bool isRunOnTye = true)
        {
            if (isRunOnTye)
            {
                return;
            }

            IServiceProvider serviceProvider = builder.Services;
            IHttpContextAccessor httpContext = serviceProvider.GetService<IHttpContextAccessor>();
            IConfiguration config = serviceProvider.GetService<IConfiguration>();
            bool fluentdEnabled = config.GetValue("Logging:FluentdEnabled", false);
            bool fileEnabled = config.GetValue("Logging:FileEnabled", false);

            LoggerConfiguration loggerConfig = new LoggerConfiguration()
                .WriteTo.Console()
                .ReadFrom.Configuration(config, "Logging")
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", config.GetValue<string>("App:Name"))
                .Enrich.WithTraceId(httpContext);

            if (fileEnabled)
            {
                loggerConfig.WriteTo.File(
                    $".\\{builder.Environment.WebRootPath}\\logs\\log_.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 1L * 1024 * 1024 * 512, // max size 0.5gb
                    shared: true,
                    outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level} ] [{TraceId}] {SourceContext} {NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");
            }

            if (!fluentdEnabled)
            {
                // push it directly to ElasticSearch Url endpoint
                loggerConfig
                    .WriteTo.Elasticsearch(config.GetValue<string>("Logging:EsUrl"))
                    .WriteTo.Console(
                        outputTemplate:
                        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}");
            }
            else
            {
                loggerConfig.WriteTo.Console(new ElasticsearchJsonFormatter());
            }

            Log.Logger = loggerConfig.CreateLogger();
        }

        internal static LoggerConfiguration WithTraceId(
            this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration,
            IHttpContextAccessor httpContextAccessor)
        {
            if (loggerEnrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerEnrichmentConfiguration));
            }

            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            return loggerEnrichmentConfiguration.With(new TraceIdEnricher(httpContextAccessor));
        }
    }
}
