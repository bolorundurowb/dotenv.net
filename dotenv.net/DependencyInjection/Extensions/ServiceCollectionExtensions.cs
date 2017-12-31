using System;
using dotenv.net.DependencyInjection.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace dotenv.net.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEnv(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddEnv(this IServiceCollection services, Action<DotEnvOptions> setupAction)
        {
            
        }
    }
}