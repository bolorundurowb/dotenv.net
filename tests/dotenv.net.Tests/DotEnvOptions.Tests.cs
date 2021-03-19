using System;
using System.Text;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests
{
    public class DotEnvOptionsTests
    {
        [Fact]
        public void ConstructorShouldInitializeWithDefaults()
        {
            var options = new DotEnvOptions();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ConstructorShouldInitializeWithSpecifiedValues()
        {
            var filePaths = new[] {"test.env"};
            var options = new DotEnvOptions(encoding: Encoding.UTF32, trimValues: false, probeForEnv: true,
                probeLevelsToSearch: 5, overwriteExistingVars: false, ignoreExceptions: false, envFilePaths: filePaths);

            options.IgnoreExceptions
                .Should()
                .BeFalse();
            options.Encoding
                .Should()
                .Be(Encoding.UTF32);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeFalse();
            options.ProbeForEnv
                .Should()
                .BeTrue();
            options.ProbeLevelsToSearch
                .Should()
                .Be(5);
            options.EnvFilePaths
                .Should()
                .Contain("test.env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithExceptions()
        {
            var options = new DotEnvOptions()
                .WithExceptions();

            options.IgnoreExceptions
                .Should()
                .BeFalse();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithoutExceptions()
        {
            var options = new DotEnvOptions()
                .WithoutExceptions();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithProbeForEnv()
        {
            var options = new DotEnvOptions()
                .WithProbeForEnv(7);

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeTrue();
            options.ProbeLevelsToSearch
                .Should()
                .Be(7);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithoutProbeForEnv()
        {
            var options = new DotEnvOptions()
                .WithoutProbeForEnv();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithOverwriteExistingVars()
        {
            var options = new DotEnvOptions()
                .WithOverwriteExistingVars();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithoutOverwriteExistingVars()
        {
            var options = new DotEnvOptions()
                .WithoutOverwriteExistingVars();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeFalse();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithTrimValues()
        {
            var options = new DotEnvOptions()
                .WithTrimValues();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeTrue();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithoutTrimValues()
        {
            var options = new DotEnvOptions()
                .WithoutTrimValues();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithEncoding()
        {
            var options = new DotEnvOptions()
                .WithEncoding(Encoding.Latin1);

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.Latin1);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithDefaultEncoding()
        {
            var options = new DotEnvOptions()
                .WithDefaultEncoding();

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }

        [Fact]
        public void ShouldGenerateOptionsWithEnvFiles()
        {
            var envFiles = new[] {"test.env", "other.env"};
            var options = new DotEnvOptions()
                .WithEnvFiles(envFiles);

            options.IgnoreExceptions
                .Should()
                .BeTrue();
            options.Encoding
                .Should()
                .Be(Encoding.UTF8);
            options.TrimValues
                .Should()
                .BeFalse();
            options.OverwriteExistingVars
                .Should()
                .BeTrue();
            options.ProbeForEnv
                .Should()
                .BeFalse();
            options.ProbeLevelsToSearch
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .BeEquivalentTo(envFiles);
        }

        [Fact]
        public void ShouldGenerateOptionsRead()
        {
            var envFiles = new[] {"quotations.env"};
            var values = new DotEnvOptions()
                .WithEnvFiles(envFiles)
                .Read();

            values.Count
                .Should()
                .BeGreaterThan(0);
        }

        [Fact]
        public void ShouldGenerateOptionsLoad()
        {
            var envFiles = new[] {"quotations.env"};
            var action = new Action(() => new DotEnvOptions()
                .WithEnvFiles(envFiles)
                .Load());

            action.Should()
                .NotThrow();
        }
    }
}