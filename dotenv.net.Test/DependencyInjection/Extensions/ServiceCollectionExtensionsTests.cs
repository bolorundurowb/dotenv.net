using System;
using System.Text;
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
        
        [Fact]
        public void ShouldWorkWhenSetupActionIsValid()
        {
            var services = new ServiceCollection();
            Action action = () => ServiceCollectionExtensions.AddEnv(services, options =>
            {
                options.AddEnvFile(".env");
                    options.AddEncoding(Encoding.UTF8);
                    options.AddThrowOnError(false);
                });
            action.Should()
                .NotThrow<Exception>();
        }
        
        [Fact]
        public void ShouldWorkWithAutomatedOptions()
        {
            var services = new ServiceCollection();
            Action action = () => ServiceCollectionExtensions.AddEnv(services);
            // NOTE: this is because the default file does not exist
            action.Should()
                .NotThrow<Exception>();
        }
    }
}