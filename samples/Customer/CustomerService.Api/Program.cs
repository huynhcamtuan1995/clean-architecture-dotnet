using CustomerService.Infrastructure;
using Microsoft.AspNetCore.Builder;
using ApiAnchor = CustomerService.Application.V1.Anchor;

namespace CustomerService.Application
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


