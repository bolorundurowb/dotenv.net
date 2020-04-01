using System;
using System.Text;
using Autofac;

namespace dotenv.net.DependencyInjection.Autofac
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Add the environment variables using autofac
        /// </summary>
        /// <param name="containerBuilder">The container builder instance</param>
        /// <param name="action">The env builder action</param>
        /// <returns>The container builder instance</returns>
        /// <exception cref="ArgumentNullException">If the container builder or action is null</exception>
        public static ContainerBuilder AddEnv(this ContainerBuilder containerBuilder,
            Action<DotEnvOptionsBuilder> action)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var dotEnvOptionsBuilder = new DotEnvOptionsBuilder();
            action(dotEnvOptionsBuilder);

            var dotEnvOptions = dotEnvOptionsBuilder.Build();
            DotEnv.Config(dotEnvOptions);

            return containerBuilder;
        }

        /// <summary>
        /// Add the environment variables using autofac
        /// </summary>
        /// <param name="containerBuilder">The container builder instance</param>
        /// <returns>The container builder instance</returns>
        /// <exception cref="ArgumentNullException">If the container builder is null</exception>
        public static ContainerBuilder AddEnv(this ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            AddEnv(containerBuilder, ac =>
            {
                ac.AddEnvFile(".env")
                    .AddEncoding(Encoding.UTF8)
                    .AddThrowOnError(true)
                    .AddTrimOptions(true);
            });

            return containerBuilder;
        }

        /// <summary>
        /// Add autofac IoC for the env reader
        /// </summary>
        /// <param name="containerBuilder">The container builder instance</param>
        /// <returns>The container builder instance</returns>
        /// <exception cref="ArgumentNullException">If the container builder is null</exception>
        public static ContainerBuilder AddEnvReader(this ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }

            containerBuilder.RegisterType<EnvReader>().As<IEnvReader>();
            return containerBuilder;
        }
    }