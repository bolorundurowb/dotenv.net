using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests
{
    public class DotEnvTests
    {
        private const string WhitespacesEnvFileName = "values-with-whitespaces.env";
        private const string WhitespacesCopyEnvFileName = "values-with-whitespaces-too.env";
        private const string ValuesAndCommentsEnvFileName = "values-and-comments.env";
        private const string NonExistentEnvFileName = "non-existent.env";
        private const string QuotationsEnvFileName = "quotations.env";

        [Fact]
        public void Config_ShouldInitializeEnvOptions_WithDefaultOptions()
        {
            var config = DotEnv.Config();

            config.Encoding
                .Should()
                .Be(Encoding.UTF8);
        }

        [Fact]
        public void ShouldReturnValidValuesWithAutoConfig()
        {
           var success = DotEnv.AutoConfig();
           
           success.Should().BeTrue();
            Environment.GetEnvironmentVariable("hello").Should().Be("world");
        }
    }
}