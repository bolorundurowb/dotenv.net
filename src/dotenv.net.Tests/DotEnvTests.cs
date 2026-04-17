using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvTests
{
    [Fact]
    public void Read_ComplexExistingEnv_ShouldExtractValidValues()
    {
        var options = new DotEnvOptions(trimValues: true, probeForEnv: true, probeLevelsToSearch: 5);
        var values = DotEnv.Read(options);

        values.Count.ShouldBe(11);
        values.ShouldContainKeyAndValue("lower_case_key", "world");
        values.ShouldContainKeyAndValue("DOUBLE_QUOTES", "double");
        values.ShouldContainKeyAndValue("SINGLE_QUOTES", "single");
        values.ShouldContainKeyAndValue("BOOLEAN", "true");
        values.ShouldContainKeyAndValue("NUMERIC", "34.56");
        values.ShouldContainKeyAndValue("DOTTED.KEY", "spaced value");
        values.ShouldContainKeyAndValue("KeyWithNoValue", string.Empty);
        values.ShouldContainKeyAndValue("DOUBLE_QUOTE_EVEN_MORE_LINES",
            $"""this{Environment.NewLine}is{Environment.NewLine}"a{Environment.NewLine}multi-line{Environment.NewLine}  value""");
        values.ShouldContainKeyAndValue("OidcAuthentication:ClientId", "your-client-id");
        values.ShouldContainKeyAndValue("OidcAuthentication:ClientSecret", "your-client-secret");
    }

    [Fact]
    public void Read_FromSingleStream_ShouldParseVariables()
    {
        using var stream = new MemoryStream("FOO=bar\nBAZ=qux"u8.ToArray());
        var options = new DotEnvOptions().WithEnvStreams(stream);
        var values = DotEnv.Read(options);

        values.ShouldContainKeyAndValue("FOO", "bar");
        values.ShouldContainKeyAndValue("BAZ", "qux");
    }

    [Fact]
    public void Read_FromStreamViaFluent_ShouldParseVariables()
    {
        using var stream = new MemoryStream("X=1"u8.ToArray());
        var values = DotEnv.Fluent().WithEnvStreams(stream).Read();

        values.ShouldContainKeyAndValue("X", "1");
    }

    [Fact]
    public void Read_FromMultipleStreams_WithoutOverwrite_ShouldPreferFirstValue()
    {
        using var first = new MemoryStream("KEY=first"u8.ToArray());
        using var second = new MemoryStream("KEY=second"u8.ToArray());
        var options = new DotEnvOptions(overwriteExistingVars: false).WithEnvStreams(first, second);
        var values = DotEnv.Read(options);

        values["KEY"].ShouldBe("first");
    }

    [Fact]
    public void Read_FromMultipleStreams_WithOverwrite_ShouldUseLastValue()
    {
        using var first = new MemoryStream("KEY=first"u8.ToArray());
        using var second = new MemoryStream("KEY=second"u8.ToArray());
        var options = new DotEnvOptions(overwriteExistingVars: true).WithEnvStreams(first, second);
        var values = DotEnv.Read(options);

        values["KEY"].ShouldBe("second");
    }

    [Fact]
    public void Read_FromStream_WithExportSyntax_WhenSupported_ShouldParse()
    {
        using var stream = new MemoryStream("export KEY=value"u8.ToArray());
        var options = new DotEnvOptions(supportExportSyntax: true).WithEnvStreams(stream);
        var values = DotEnv.Read(options);

        values.ShouldContainKeyAndValue("KEY", "value");
    }

    [Fact]
    public void Constructor_WithEnvStreams_ShouldExposeStreamsOnOptions()
    {
        using var stream = new MemoryStream("K=v"u8.ToArray());
        var options = new DotEnvOptions(envStreams: new[] { stream });

        options.EnvStreams.ShouldNotBeNull();
        options.EnvStreams!.Single().ShouldBe(stream);
    }
}