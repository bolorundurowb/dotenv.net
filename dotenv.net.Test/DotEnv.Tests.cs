using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test
{
    public class DotEnvTests
    {
        private const string whitespacesEnvFileName = "values-with-whitespaces.env";
        private const string valuesAndCommentsEnvFileName = "values-and-comments.env";
        private const string nonExistentEnvFileName = "non-existent.env";

        [Fact]
        public void ThrowsExceptionWithNonExistentEnvFileWhenThrowErrorIsTrue()
        {
            Action action = () => DotEnv.Config(true, nonExistentEnvFileName);
            action.ShouldThrowExactly<FileNotFoundException>()
                .WithMessage($"An environment file with path \"{nonExistentEnvFileName}\" does not exist.");
        }

        [Fact]
        public void DoesNotThrowExceptionWithNonExistentEnvFileWhenThrowErrorIsFalse()
        {
            Action action = () => DotEnv.Config(false, "non-existent.env");
            action.ShouldNotThrow();
        }

        [Fact]
        public void AddsEnvironmentVariablesIfADefaultEnvFileExists()
        {
            Action action = () => DotEnv.Config();
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("hello").Should().Be("world");
        }

        [Fact]
        public void AddsEnvironmentVariablesAndSetsValueAsNullIfNoneExists()
        {
            Action action = () => DotEnv.Config();
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("strongestavenger").Should().Be(null);
        }

        [Fact]
        public void AllowsEnvFilePathToBeSpecified()
        {
            Action action = () => DotEnv.Config(true, valuesAndCommentsEnvFileName);
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("me").Should().Be("winner");
        }

        [Fact]
        public void ShouldReturnUntrimmedValuesWhenTrimIsFalse()
        {
            DotEnv.Config(true, whitespacesEnvFileName, Encoding.UTF8, false);

            Environment.GetEnvironmentVariable("DB_CONNECTION").Should().Be("mysql  ");
            Environment.GetEnvironmentVariable("DB_HOST").Should().Be("127.0.0.1");
            Environment.GetEnvironmentVariable("DB_PORT").Should().Be("  3306");
            Environment.GetEnvironmentVariable("DB_DATABASE").Should().Be("laravel");
        }

        [Fact]
        public void ShouldReturnTrimmedValuesWhenTrimIsTrue()
        {
            DotEnv.Config(true, whitespacesEnvFileName, Encoding.UTF8, true);

            Environment.GetEnvironmentVariable("DB_CONNECTION").Should().Be("mysql");
            Environment.GetEnvironmentVariable("DB_HOST").Should().Be("127.0.0.1");
            Environment.GetEnvironmentVariable("DB_PORT").Should().Be("3306");
            Environment.GetEnvironmentVariable("DB_DATABASE").Should().Be("laravel");
        }
    }
}