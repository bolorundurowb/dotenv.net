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
            
            envReader.GetValue("CONNECTION").Should().Be("mysql");
            envReader.TryGetValue("NON_EXISTENT_KEY", out var _).Should().BeFalse();
            envReader.TryGetValue("DATABASE", out var database).Should().BeTrue();
            database.Should().Be("laravel");
        }

        [Fact]
        public void ShouldReadTypedValuesWithReaderMethods()
        {
            DotEnv.Config(true, ValueTypesEnvFileName, Encoding.UTF8);
            var envReader = new EnvReader();
            
            envReader.GetValue<int>("PORT").Should().Be(3306);
            envReader.TryGetValue<decimal>("HOST", out var _).Should().BeFalse();
            envReader.TryGetValue<bool>("IS_PRESENT", out var isPresent).Should().BeTrue();
            isPresent.Should().BeTrue();
        }
    }
}