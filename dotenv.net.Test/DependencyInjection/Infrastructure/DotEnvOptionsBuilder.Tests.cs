using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test.DependencyInjection.Infrastructure
{
    public class DotEnvOptionsBuilderTests
    {
        [Fact]
        public void ShouldGenerateSpecified()
        {
            var fileName = "file.env";
            var dotEnvOptionsBuilder = new DotEnvOptionsBuilder();

           var dotEnvOptions = dotEnvOptionsBuilder
                .AddEncoding(Encoding.UTF32)
                .AddEnvFile(fileName)
                .AddThrowOnError(false)
                .AddTrimOptions(true)
                .Build();

           dotEnvOptions.Encoding.Should().Be(Encoding.UTF32);
           dotEnvOptions.EnvFile.Should().Be(fileName);
           dotEnvOptions.TrimValues.Should().BeTrue();
           dotEnvOptions.ThrowOnError.Should().BeFalse();
        }
    }
}