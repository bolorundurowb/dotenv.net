using System;
using System.IO;
using System.Text;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvFluentTests
{
    private const string WhitespacesEnvFileName = "whitespaces.env";
    private const string NonExistentEnvFileName = "non-existent.env";
    private const string QuotationsEnvFileName = "quotations.env";
    private const string MultiLinesEnvFileName = "multi-lines.env";
    private const string AsciiEnvFileName = "ascii.env";
    private const string GenericEnvFileName = "generic.env";
    private const string IncompleteEnvFileName = "incomplete.env";

    [Fact]
    public void ConfigShouldThrowWithNonExistentEnvAndTrackedExceptions()
    {
        var action = new Action(() => DotEnv.Fluent()
            .WithExceptions()
            .WithEnvFiles(NonExistentEnvFileName)
            .Load());

        action.Should()
            .ThrowExactly<FileNotFoundException>();
    }

    [Fact]
    public void ConfigShouldLoadEnvWithProvidedEncoding()
    {
        DotEnv.Fluent()
            .WithEncoding(Encoding.ASCII)
            .WithEnvFiles(AsciiEnvFileName)
            .Load();

        EnvReader.GetStringValue("ENCODING")
            .Should()
            .Be("ASCII");
    }

    [Fact]
    public void ConfigShouldLoadEnvWithTrimOptions()
    {
        DotEnv.Fluent()
            .WithEnvFiles(WhitespacesEnvFileName)
            .WithTrimValues()
            .Load();

        EnvReader.GetStringValue("DB_DATABASE")
            .Should()
            .Be("laravel");

        DotEnv.Fluent()
            .WithEnvFiles(WhitespacesEnvFileName)
            .WithoutTrimValues()
            .Load();

        EnvReader.GetStringValue("DB_DATABASE")
            .Should()
            .Be(" laravel  ");
    }

    [Fact]
    public void ConfigShouldLoadEnvWithExistingVarOverwriteOptions()
    {
        Environment.SetEnvironmentVariable("Generic", "Existing");

        DotEnv.Fluent()
            .WithEnvFiles(GenericEnvFileName)
            .WithoutOverwriteExistingVars()
            .Load();

        EnvReader.GetStringValue("Generic")
            .Should()
            .Be("Existing");

        DotEnv.Fluent()
            .WithEnvFiles(GenericEnvFileName)
            .WithOverwriteExistingVars()
            .Load();

        EnvReader.GetStringValue("Generic")
            .Should()
            .Be("Value");
    }

    [Fact]
    public void ConfigShouldLoadDefaultEnvWithProbeOptions()
    {
        var action = new Action(() => DotEnv.Fluent()
            .WithProbeForEnv(2)
            .WithExceptions()
            .Load());

        action.Should()
            .ThrowExactly<FileNotFoundException>();

        action = () => DotEnv.Fluent()
            .WithProbeForEnv(5)
            .WithExceptions()
            .Load();

        action.Should()
            .NotThrow();

        EnvReader.GetStringValue("hello")
            .Should()
            .Be("world");
    }

    [Fact]
    public void ConfigShouldLoadEnvWithQuotedValues()
    {
        DotEnv.Fluent()
            .WithEnvFiles(QuotationsEnvFileName)
            .WithTrimValues()
            .Load();

        EnvReader.GetStringValue("DOUBLE_QUOTES")
            .Should()
            .Be("double");
        EnvReader.GetStringValue("SINGLE_QUOTES")
            .Should()
            .Be("single");
    }

    [Fact]
    public void ConfigLoadsMultilineEnvs()
    {
        DotEnv.Fluent()
            .WithEnvFiles(MultiLinesEnvFileName)
            .WithTrimValues()
            .Load();

        EnvReader.GetStringValue("DOUBLE_QUOTE")
            .Should()
            .Be("double");
        EnvReader.GetStringValue("DOUBLE_QUOTE_MULTI_LINE")
            .Should()
            .Be($"dou{Environment.NewLine}bler");
        EnvReader.GetStringValue("DOUBLE_QUOTE_EVEN_MORE_LINES")
            .Should()
            .Be($"dou{Environment.NewLine}{Environment.NewLine}b{Environment.NewLine}{Environment.NewLine}lest");
        EnvReader.GetStringValue("DOUBLE_QUOTE_WHITESPACE_LINES")
            .Should()
            .Be($"dou{Environment.NewLine}{Environment.NewLine}b{Environment.NewLine}{Environment.NewLine}lest");
        EnvReader.GetStringValue("DOUBLE_QUOTE.WITH_DOTS")
            .Should()
            .Be($"dou{Environment.NewLine}{Environment.NewLine}b{Environment.NewLine}{Environment.NewLine}le{Environment.NewLine}dots");
        EnvReader.GetStringValue("DOUBLE_QUOTE_NO_CLOSE")
            .Should()
            .Be($"\"dou{Environment.NewLine}{Environment.NewLine}b{Environment.NewLine}{Environment.NewLine}lest the more");
    }

    [Fact]
    public void ConfigShouldLoadEnvWithInvalidEnvEntries()
    {
        DotEnv.Fluent()
            .WithEnvFiles(IncompleteEnvFileName)
            .WithoutTrimValues()
            .Load();

        EnvReader.HasValue("KeyWithNoValue")
            .Should()
            .BeFalse();
    }
}
