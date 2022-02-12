using Microsoft.AspNetCore.Builder;
using ProductService.Infrastructure;
using ApiAnchor = ProductService.Application.V1.Anchor;

namespace ProductService.Application
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
