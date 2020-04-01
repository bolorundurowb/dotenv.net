using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace dotenv.net.DependencyInjection.Microsoft
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
                    .AddEncoding(Encoding.UTF8)
                    .AddThrowOnError(true)
                    .AddTrimOptions(true);
            });
            return services;
        }

        /// <summary>
        /// Add the environment vars using a service
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="setupAction">The dot env options builder action</param>
        /// <returns>The service collection</returns>
        /// <exception cref="ArgumentNullException">If the service passed in is null</exception>
        public static IServiceCollection AddEnv(this IServiceCollection services,
            Action<DotEnvOptionsBuilder> setupAction)
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

        /// <summary>
        /// Use the env reader class ad the provider for reading environment variables
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>The service collection</returns>
        /// <exception cref="ArgumentNullException">If the service passed in is null</exception>
        public static IServiceCollection AddEnvReader(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IEnvReader, EnvReader>();
            return services;
        }
}