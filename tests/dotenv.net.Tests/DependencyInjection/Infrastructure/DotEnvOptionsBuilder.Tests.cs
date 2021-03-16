using System.Text;
using Xunit;

namespace dotenv.net.Tests.DependencyInjection.Infrastructure
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
                .AddIgnoreExceptionOptions(false)
                .AddTrimOptions(true)
                .Build();

           dotEnvOptions.Encoding.Should().Be(Encoding.UTF32);
           dotEnvOptions.EnvFilePaths.Should().Be(fileName);
           dotEnvOptions.TrimValues.Should().BeTrue();
           dotEnvOptions.IgnoreExceptions.Should().BeFalse();
        }
    }
}