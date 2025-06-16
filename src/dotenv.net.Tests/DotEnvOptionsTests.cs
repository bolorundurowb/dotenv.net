using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvOptionsTests
{
    [Fact]
    public void Constructor_WithNullEnvFilePaths_ShouldUseDefaultPath()
    {
        var options = new DotEnvOptions(envFilePaths: null);
        options.EnvFilePaths.Should().ContainInOrder(".env");
    }

    [Fact]
    public void Constructor_WithEmptyEnvFilePaths_ShouldUseDefaultPath()
    {
        var emptyPaths = new List<string>();
        var options = new DotEnvOptions(envFilePaths: emptyPaths);
        options.EnvFilePaths.Should().ContainInOrder(DotEnvOptions.DefaultEnvFileName);
    }

    [Fact]
    public void Constructor_WithNullEncoding_ShouldUseUtf8()
    {
        var options = new DotEnvOptions(encoding: null);
        options.Encoding.Should().Be(Encoding.UTF8);
    }

    [Fact]
    public void WithEncoding_WithNullEncoding_ShouldUseUtf8()
    {
        var options = new DotEnvOptions();
        options.WithEncoding(null!);
        options.Encoding.Should().Be(Encoding.UTF8);
    }

    [Fact]
    public void WithEnvFiles_WithNullParams_ShouldUseDefaultPath()
    {
        var options = new DotEnvOptions();
        options.WithEnvFiles(null!);
        options.EnvFilePaths.Should().ContainInOrder(DotEnvOptions.DefaultEnvFileName);
    }

    [Fact]
    public void WithEnvFiles_WithEmptyParams_ShouldUseDefaultPath()
    {
        var options = new DotEnvOptions();
        options.WithEnvFiles(Array.Empty<string>());
        options.EnvFilePaths.Should().ContainInOrder(DotEnvOptions.DefaultEnvFileName);
    }

    [Fact]
    public void WithProbeForEnv_WithNegativeProbeLevels_ShouldUseDefaultProbeDepth()
    {
        var options = new DotEnvOptions();
        options.WithProbeForEnv(-1);
        options.ProbeLevelsToSearch.Should().Be(DotEnvOptions.DefaultProbeAscendLimit);
    }

    [Fact]
    public void WithoutProbeForEnv_ShouldResetProbeLevelsToDefault()
    {
        var options = new DotEnvOptions().WithProbeForEnv(10);
        options.WithoutProbeForEnv();
        options.ProbeForEnv.Should().BeFalse();
        options.ProbeLevelsToSearch.Should().Be(DotEnvOptions.DefaultProbeAscendLimit);
    }

    [Fact]
    public void WithDefaultEncoding_ShouldResetToUtf8()
    {
        var options = new DotEnvOptions().WithEncoding(Encoding.ASCII);
        options.WithDefaultEncoding();
        options.Encoding.Should().Be(Encoding.UTF8);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithExceptions_ShouldSetIgnoreExceptionsCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(ignoreExceptions: initialValue);
        options.WithExceptions();
        options.IgnoreExceptions.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithoutExceptions_ShouldSetIgnoreExceptionsCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(ignoreExceptions: initialValue);
        options.WithoutExceptions();
        options.IgnoreExceptions.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithOverwriteExistingVars_ShouldSetOverwriteCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(overwriteExistingVars: initialValue);
        options.WithOverwriteExistingVars();
        options.OverwriteExistingVars.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithoutOverwriteExistingVars_ShouldSetOverwriteCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(overwriteExistingVars: initialValue);
        options.WithoutOverwriteExistingVars();
        options.OverwriteExistingVars.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithTrimValues_ShouldSetTrimValuesCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(trimValues: initialValue);
        options.WithTrimValues();
        options.TrimValues.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WithoutTrimValues_ShouldSetTrimValuesCorrectly(bool initialValue)
    {
        var options = new DotEnvOptions(trimValues: initialValue);
        options.WithoutTrimValues();
        options.TrimValues.Should().BeFalse();
    }
}
