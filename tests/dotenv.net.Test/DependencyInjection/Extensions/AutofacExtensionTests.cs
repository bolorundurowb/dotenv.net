using System;
using Autofac;
using dotenv.net.DependencyInjection.Autofac;
using dotenv.net.Interfaces;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test.DependencyInjection.Extensions
{
    public class AutofacExtensionTests
    {
        [Fact]
        public void ShouldThrowWhenContainerBuilderIsNull()
        {
            Action action = () => AutofacExtensions.AddEnv(null, builder => { });
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ShouldThrowWhenSetupActionIsNull()
        {
            var containerBuilder = new ContainerBuilder();
            Action action = () => containerBuilder.AddEnv(null);
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void AddsEnvironmentVariablesIfADefaultEnvFileExists()
        {
            var builder = new ContainerBuilder();
            Action action = () => builder.AddEnv();

            action.Should()
                .NotThrow<Exception>();
            Environment.GetEnvironmentVariable("hello").Should().Be("world");
        }

        [Fact]
        public void ShouldThrowWhenContainerBuilderIsNotProvided()
        {
            Action action = () => AutofacExtensions.AddEnvReader(null);
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ShouldInjectEnvReaderWhenContainerBuilderServicesIsProvided()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.AddEnvReader();
            var provider = containerBuilder.Build();

            using (var scope = provider.BeginLifetimeScope())
            {
                object service = null;
                Action action = () => service = scope.Resolve<IEnvReader>();

                action.Should()
                    .NotThrow<Exception>();
                service.Should()
                    .NotBeNull();
                service.Should()
                    .BeOfType<EnvReader>();
            }
        }
    }
}