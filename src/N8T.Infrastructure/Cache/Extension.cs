using System;
using Microsoft.Extensions.DependencyInjection;

namespace N8T.Infrastructure.Cache
{
    public static class Extension
    {
        public static IServiceCollection AddDefaultCache(this IServiceCollection services,
            Action<IServiceCollection> doMoreActions = null)
        {
            services.AddSingleton(typeof(ICacheService<>), typeof(MemoryService<>));

            doMoreActions?.Invoke(services);

            return services;
        }
    }
}
