using System;
using dotenv.net.DependencyInjection.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dotenv.net.Test.DependencyInjection.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ShouldThrowWhenServicesAreNull()
        {
            Action action = () => ServiceCollectionExtensions.AddEnv(null, builder => { });
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }
        
        [Fact]
        public void ShouldThrowWhenSetupActionIsNull()
        {
            var services = new ServiceCollection();
            Action action = () => ServiceCollectionExtensions.AddEnv(services, null);
            action.Should()
                .ThrowExactly<ArgumentNullException>();
        }
    }
}