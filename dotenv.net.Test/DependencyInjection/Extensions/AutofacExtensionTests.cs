using System;
using Autofac;
using dotenv.net.DependencyInjection.Extensions;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test.DependencyInjection.Extensions
{
    public class AutofacExtensionTests
    {
        [Fact]
        public void ShouldThrowWhenContainerBuilderIsNull()
        {
            Action action = () => AutoFacExtension.AddEnv(null, builder => { });
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
    }
}
