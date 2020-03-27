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
        public void AddsEnvironmentVariablesIfADefaultEnvFileExists()
        {
            SetupTest();
            Environment.GetEnvironmentVariable("hello").Should().Be("world");
        }

        private static void SetupTest()
        {
            var builder = new ContainerBuilder();
            builder.AddEnv();
        }
    }
}
