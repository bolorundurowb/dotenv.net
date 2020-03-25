using Autofac;
using dotenv.net.DependencyInjection.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace dotenv.net.Test.DependencyInjection
{
    public class AutofacExtensionTests
    {
        [Fact]
        public void AddsEnvironmentVariablesIfADefaultEnvFileExists()
        {
            SetupTest();
            Environment.GetEnvironmentVariable("hello").Should().Be("world");
        }

        private void SetupTest()
        {
            var builder = new ContainerBuilder();
            builder.AddEnv();
        }
    }
}
