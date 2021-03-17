using System.Text;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests
{
    public class DotEnvOptionsTests
    {
        [Fact]
        public void Constructor_ShouldInitialize_WithDefaults()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void Constructor_ShouldInitialize_WithSpecifiedValues()
        {
            var filePaths = new[] {"test.env"};
            var options = new DotEnvOptions(encoding: Encoding.UTF32, trimValues: false, probeForEnv: true, probeDirectoryDepth: 5, overwriteExistingVars: false, ignoreExceptions: false, envFilePaths: filePaths);

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
            options.ProbeDirectoryDepth
                .Should()
                .Be(5);
            options.EnvFilePaths
                .Should()
                .Contain("test.env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithExceptions()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithoutExceptions()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithProbeForEnv()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(7);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithoutProbeForEnv()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
    }
}