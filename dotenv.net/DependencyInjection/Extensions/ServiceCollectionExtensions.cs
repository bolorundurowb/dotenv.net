using System;
using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace dotenv.net.DependencyInjection.Extensions
{
    /// <summary>
    /// Servcie collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the environment vars using a service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
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

        /// <summary>
        /// Add the environment vars using a service
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="setupAction">The dot env options buider action</param>
        /// <returns>The service collection</returns>
        /// <exception cref="ArgumentNullException">If the service passed in is null</exception>
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
            
            var dotEnvOptionsBuilder = new DotEnvOptionsBuilder();
            setupAction(dotEnvOptionsBuilder);
            
            var dotEnvOptions = dotEnvOptionsBuilder.Build();
            DotEnv.Config(dotEnvOptions);
            
            return services;
        }
    }
}