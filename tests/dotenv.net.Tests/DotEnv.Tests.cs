using System;
using System.IO;
using System.Text;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests
{
    public class DotEnvTests
    {
        private const string WhitespacesEnvFileName = "whitespaces.env";
        private const string WhitespacesCopyEnvFileName = "values-with-whitespaces-too.env";
        private const string ValuesAndCommentsEnvFileName = "values-and-comments.env";
        private const string NonExistentEnvFileName = "non-existent.env";
        private const string QuotationsEnvFileName = "quotations.env";
        private const string AsciiEnvFileName = "ascii.env";

        [Fact]
        public void ConfigShouldThrowWithNonExistentEnvAndTrackedExceptions()
        {
            var action = new Action(() => DotEnv.Config(new DotEnvOptions(ignoreExceptions: false, envFilePaths: new [] {NonExistentEnvFileName})));

            action.Should()
                .ThrowExactly<FileNotFoundException>();
        }

        [Fact]
        public void ConfigShouldLoadEnvWithProvidedEncoding()
        {
            DotEnv.Config(new DotEnvOptions(envFilePaths: new [] {AsciiEnvFileName}, encoding: Encoding.ASCII));

            EnvReader.GetStringValue("ENCODING")
                .Should()
                .Be("ASCII");
        }

        [Fact]
        public void ConfigShouldLoadEnvWithTrimOptions()
        {
            DotEnv.Config(new DotEnvOptions(envFilePaths: new [] {WhitespacesEnvFileName}, trimValues: true));

            EnvReader.GetStringValue("DB_DATABASE")
                .Should()
                .Be("laravel");
            
            DotEnv.Config(new DotEnvOptions(envFilePaths: new [] {WhitespacesEnvFileName}, trimValues: false));

            EnvReader.GetStringValue("DB_DATABASE")
                .Should()
                .Be(" laravel  ");
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
            values["DB_CONNECTION"]
                .Should()
                .Be("mysql");
            values["DB_PORT"]
                .Should()
                .Be("3306");
            values["DB_HOST"]
                .Should()
                .Be("127.0.0.1");
            values["DB_DATABASE"]
                .Should()
                .Be("laravel");
            values["IS_PRESENT"]
                .Should()
                .Be("true");
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