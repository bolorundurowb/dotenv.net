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
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithOverwriteExistingVars()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithoutOverwriteExistingVars()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithTrimValues()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithoutTrimValues()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithEncoding()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithDefaultEncoding()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .Contain(".env");
        }
        
        [Fact]
        public void WithExceptions_ShouldGenerateOptions_WithEnvFiles()
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
            options.ProbeDirectoryDepth
                .Should()
                .Be(4);
            options.EnvFilePaths
                .Should()
                .BeEquivalentTo(envFiles);
        }
    }
}