using System;
using System.IO;
using System.Text;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests
{
    public class DotEnvFluentTests
    {
        private const string WhitespacesEnvFileName = "whitespaces.env";
        private const string WhitespacesCopyEnvFileName = "values-with-whitespaces-too.env";
        private const string ValuesAndCommentsEnvFileName = "values-and-comments.env";
        private const string NonExistentEnvFileName = "non-existent.env";
        private const string QuotationsEnvFileName = "quotations.env";

        [Fact]
        public void Config_ShouldInitializeEnvOptions_WithDefaultOptions()
        {
            var config = DotEnv.Fluent();

            config.Encoding
                .Should()
                .Be(Encoding.UTF8);
        }

        [Fact]
        public void Config_ShouldNotLoadEnv_WithDefaultOptions_AsThereIsNoEnvFile()
        {
            var action = new Action(() => DotEnv.Config(new DotEnvOptions(ignoreExceptions: false)));

            action.Should()
                .ThrowExactly<FileNotFoundException>();
        }

        [Fact]
        public void Config_ShouldLoadEnv_WithProbeEnvOptions()
        {
            DotEnv.Config(new DotEnvOptions(probeForEnv: true));

            EnvReader.GetStringValue("hello")
                .Should()
                .Be("world");
        }

        [Fact]
        public void AutoConfig_ShouldLocateAndLoadEnv()
        {
            var success = DotEnv.AutoConfig();

            success.Should().BeTrue();
            EnvReader.GetStringValue("uniquekey")
                .Should()
                .Be("kjdjkd");
        }

        [Fact]
        public void Read_Should_ReturnTheReadValues()
        {
            var values =
                DotEnv.Read(new DotEnvOptions(trimValues: true, envFilePaths: new[] {WhitespacesEnvFileName}));

            values.Count
                .Should()
                .BeGreaterThan(0);
            values["DB_DATABASE"]
                .Should()
                .Be("laravel");
        }

        [Fact]
        public void Read_Should_ThrowAnException_WithEmptyFileNameAndConfig()
        {
            var action = new Action(() =>
                DotEnv.Read(new DotEnvOptions(ignoreExceptions: false, envFilePaths: new[] {string.Empty})));

            action.Should()
                .ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Load_Should_IgnoreFieldsThatHaveExistingValues_WithConfig()
        {
            Environment.SetEnvironmentVariable("me", "whoIam");
            DotEnv.Load(new DotEnvOptions(overwriteExistingVars: false,
                envFilePaths: new[] {ValuesAndCommentsEnvFileName}));

            EnvReader.GetStringValue("me")
                .Should()
                .Be("whoIam");
        }
    }
}