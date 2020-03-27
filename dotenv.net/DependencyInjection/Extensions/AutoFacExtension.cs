using Autofac;
using dotenv.net.DependencyInjection.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotenv.net.DependencyInjection.Extensions
{
    public static class AutoFacExtension
    {
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
    }
}