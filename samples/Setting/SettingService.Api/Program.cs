using Microsoft.AspNetCore.Builder;
using SettingService.Infrastructure;
using ApiAnchor = SettingService.Application.V1.Anchor;

namespace SettingService.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCoreServices(builder.Configuration, builder.Environment, typeof(ApiAnchor));

            WebApplication app = builder.Build();
            app.UseCoreApplication(builder.Environment);

            app.Run();
        }
    }
}
