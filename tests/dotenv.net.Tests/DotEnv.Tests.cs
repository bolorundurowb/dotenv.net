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
        private const string NonExistentEnvFileName = "non-existent.env";
        private const string QuotationsEnvFileName = "quotations.env";
        private const string AsciiEnvFileName = "ascii.env";
        private const string GenericEnvFileName = "generic.env";
        private const string IncompleteEnvFileName = "incomplete.env";

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
        public void ConfigShouldLoadEnvWithExistingVarOverwriteOptions()
        {
            Environment.SetEnvironmentVariable("Generic", "Existing");
            
            DotEnv.Config(new DotEnvOptions(envFilePaths: new [] {GenericEnvFileName}, overwriteExistingVars: false));

            EnvReader.GetStringValue("Generic")
                .Should()
                .Be("Existing");
            
            DotEnv.Config(new DotEnvOptions(envFilePaths: new [] {GenericEnvFileName}, overwriteExistingVars: true));

            EnvReader.GetStringValue("Generic")
                .Should()
                .Be("Value");
        }

        [Fact]
        public void ConfigShouldLoadDefaultEnvWithProbeOptions()
        {
            var action = new Action(() => DotEnv.Config(new DotEnvOptions(probeForEnv: true, probeDirectoryDepth: 2, ignoreExceptions: false)));

            action.Should()
                .ThrowExactly<ArgumentException>();
            
            action = () => DotEnv.Config(new DotEnvOptions(probeForEnv: true, probeDirectoryDepth: 5, ignoreExceptions: false));

            action.Should()
                .NotThrow();

            EnvReader.GetStringValue("hello")
                .Should()
                .Be("world");
        }

        [Fact]
        public void ConfigShouldLoadEnvWithQuotedValues()
        {
            DotEnv.Config(new DotEnvOptions(envFilePaths: new [] {QuotationsEnvFileName}, trimValues: true));

            EnvReader.GetStringValue("DOUBLE_QUOTES")
                .Should()
                .Be("double");
            EnvReader.GetStringValue("SINGLE_QUOTES")
                .Should()
                .Be("single");
        }

        [Fact]
        public void ConfigShouldLoadEnvWithInvalidEnvEntries()
        {
            DotEnv.Config(new DotEnvOptions(envFilePaths: new [] {IncompleteEnvFileName}, trimValues: false));

            EnvReader.HasValue("KeyWithNoValue")
                .Should()
                .BeFalse();
        }

        [Fact]
        public void AutoConfigShouldLoadDefaultEnvWithProbeOptions()
        {
           Action action = () => DotEnv.AutoConfig(5);

            action.Should()
                .NotThrow();

            EnvReader.GetStringValue("hello")
                .Should()
                .Be("world");
        }
    }
}