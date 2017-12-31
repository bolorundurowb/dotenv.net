using System;
using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace dotenv.net.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEnv(this IServiceCollection services)
        {
            AddEnv(services, builder =>
            {
                builder
                    .AddEnvFile(".env")
                    .AddEncoding(Encoding.Default)
                    .AddThrowOnError(true);
            });
            return services;
        }

        public static IServiceCollection AddEnv(this IServiceCollection services, Action<DotEnvOptionsBuilder> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }
            
            DotEnvOptionsBuilder dotEnvOptionsBuilder = new DotEnvOptionsBuilder();
            setupAction(dotEnvOptionsBuilder);
            
            var dotEnvOptions = dotEnvOptionsBuilder.Build();
            DotEnv.Config(dotEnvOptions);
            
            return services;
        }
    }
}