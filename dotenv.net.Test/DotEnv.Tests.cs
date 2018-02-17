using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test
{
    public class DotEnvTests
    {
        [Fact]
        public void ThrowsExceptionWithNonExistentEnvFileWhenThrowErrorIsTrue()
        {
            Action action = () => DotEnv.Config(true, "hello");
            action.ShouldThrowExactly<FileNotFoundException>()
                .WithMessage("An enviroment file with path \"hello\" does not exist.");
        }
        
        [Fact]
        public void DoesNotThrowExceptionWithNonExistentEnvFileWhenThrowErrorIsFalse()
        {
            Action action = () => DotEnv.Config(false, "hello.env");
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
            Action action = () => DotEnv.Config(true, "./../../../alt.env");
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("me").Should().Be("winner");
        }
    }
}