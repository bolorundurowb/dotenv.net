using System.Text;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test.Utilities
{
    public class EnvReaderTests
    {
        private const string ValueTypesEnvFileName = "various-value-types.env";

        [Fact]
        public void ShouldReadValuesWithReaderMethods()
        {
            DotEnv.Config(true, ValueTypesEnvFileName, Encoding.UTF8);
            var envReader = new EnvReader();
            
            envReader.GetStringValue("CONNECTION").Should().Be("mysql");
            envReader.TryGetStringValue("NON_EXISTENT_KEY", out _).Should().BeFalse();
            envReader.TryGetStringValue("DATABASE", out var database).Should().BeTrue();
            database.Should().Be("laravel");
        }

        [Fact]
        public void ShouldReadTypedValuesWithReaderMethods()
        {
            DotEnv.Config(true, ValueTypesEnvFileName, Encoding.UTF8);
            var envReader = new EnvReader();
            
            envReader.GetIntValue("PORT").Should().Be(3306);
            envReader.TryGetStringValue("HOST", out _).Should().BeFalse();
            envReader.TryGetBooleanValue("IS_PRESENT", out var isPresent).Should().BeTrue();
            isPresent.Should().BeTrue();
        }
    }
}