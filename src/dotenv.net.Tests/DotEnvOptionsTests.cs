using System;
using System.Collections.Generic;
using System.Text;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvOptionsTests
{
    [Fact]
    public void Constructor_WithNullEnvFilePaths_ShouldUseDefaultPath()
    {
        var options = new DotEnvOptions(envFilePaths: null);
        options.EnvFilePaths.ShouldBe([DotEnvOptions.DefaultEnvFileName]);
    }

    [Fact]
    public void Constructor_WithEmptyEnvFilePaths_ShouldUseDefaultPath()
    {
        var emptyPaths = new List<string>();
        var options = new DotEnvOptions(envFilePaths: emptyPaths);
        options.EnvFilePaths.ShouldBe([DotEnvOptions.DefaultEnvFileName]);
    }

    [Fact]
    public void Constructor_WithNullEncoding_ShouldUseUtf8()
    {
        var options = new DotEnvOptions(encoding: null);
        options.Encoding.ShouldBe(Encoding.UTF8);
    }

    [Fact]
    public void WithEncoding_WithNullEncoding_ShouldThrowException()
    {
        var options = new DotEnvOptions();
        Action action = () => options.WithEncoding(null!);
        action.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Encoding cannot be null (Parameter 'encoding')");
    }

    [Fact]
    public void WithEnvFiles_WithNullParams_ShouldThrowException()
    {
        var options = new DotEnvOptions();
        Action action = () => options.WithEnvFiles(null!);
        action.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("EnvFilePaths cannot be null (Parameter 'envFilePaths')");
    }

    [Fact]
    public void WithEnvFiles_WithEmptyParams_ShouldUseDefaultPath()
    {
        var options = new DotEnvOptions();
        options.WithEnvFiles(Array.Empty<string>());
        options.EnvFilePaths.ShouldBe([DotEnvOptions.DefaultEnvFileName]);
    }

    [Fact]
    public void WithProbeForEnv_WithNegativeProbeLevels_ShouldUseDefaultProbeDepth()
    {
        var options = new DotEnvOptions();
        options.WithProbeForEnv(-1);
        options.ProbeLevelsToSearch.ShouldBe(DotEnvOptions.DefaultProbeAscendLimit);
    }

    [Fact]
    public void WithoutProbeForEnv_ShouldResetProbeLevelsToDefault()
    {
        var options = new DotEnvOptions().WithProbeForEnv(10);
        options.WithoutProbeForEnv();
        options.ProbeForEnv.ShouldBeFalse();
        options.ProbeLevelsToSearch.ShouldBe(DotEnvOptions.DefaultProbeAscendLimit);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithExceptions_ShouldSetIgnoreExceptionsCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(ignoreExceptions: initialValue);
        options.WithExceptions();
        options.IgnoreExceptions.ShouldBeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithoutExceptions_ShouldSetIgnoreExceptionsCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(ignoreExceptions: initialValue);
        options.WithoutExceptions();
        options.IgnoreExceptions.ShouldBeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithOverwriteExistingVars_ShouldSetOverwriteCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(overwriteExistingVars: initialValue);
        options.WithOverwriteExistingVars();
        options.OverwriteExistingVars.ShouldBeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithoutOverwriteExistingVars_ShouldSetOverwriteCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(overwriteExistingVars: initialValue);
        options.WithoutOverwriteExistingVars();
        options.OverwriteExistingVars.ShouldBeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithTrimValues_ShouldSetTrimValuesCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(trimValues: initialValue);
        options.WithTrimValues();
        options.TrimValues.ShouldBeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithoutTrimValues_ShouldSetTrimValuesCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(trimValues: initialValue);
        options.WithoutTrimValues();
        options.TrimValues.ShouldBeFalse();
    }

    [Fact]
    public void Read_ComplexExistingEnv_ShouldExtractValidValues()
    {
        var values = DotEnv.Fluent()
            .WithTrimValues()
            .WithProbeForEnv()
            .Read();

        values.ShouldContainKeyAndValue("lower_case_key", "world");
        values.ShouldContainKeyAndValue("DOUBLE_QUOTES", "double");
        values.ShouldContainKeyAndValue("SINGLE_QUOTES", "single");
        values.ShouldContainKeyAndValue("BOOLEAN", "true");
        values.ShouldContainKeyAndValue("NUMERIC", "34.56");
        values.ShouldContainKeyAndValue("DOTTED.KEY", "spaced value");
        values.ShouldContainKeyAndValue("KeyWithNoValue", string.Empty);
        values.ShouldContainKeyAndValue("DOUBLE_QUOTE_EVEN_MORE_LINES",
            $"""this{Environment.NewLine}is{Environment.NewLine}"a{Environment.NewLine}multi-line{Environment.NewLine}  value""");
    }
}
