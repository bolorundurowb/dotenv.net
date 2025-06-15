using System;
using System.IO;
using System.Text;
using dotenv.net.Utilities;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Tests;

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
        var action = new Action(() => DotEnv.Config(new DotEnvOptions(ignoreExceptions: false, envFilePaths: new[] { NonExistentEnvFileName })));

        action.Should()
            .ThrowExactly<FileNotFoundException>();
    }

    [Fact]
    public void ConfigShouldLoadEnvWithProvidedEncoding()
    {
        DotEnv.Config(new DotEnvOptions(envFilePaths: new[] { AsciiEnvFileName }, encoding: Encoding.ASCII));

        EnvReader.GetStringValue("ENCODING")
            .Should()
            .Be("ASCII");
    }

    [Fact]
    public void ConfigShouldLoadEnvWithTrimOptions()
    {
        DotEnv.Config(new DotEnvOptions(envFilePaths: new[] { WhitespacesEnvFileName }, trimValues: true));

        EnvReader.GetStringValue("DB_DATABASE")
            .Should()
            .Be("laravel");

        DotEnv.Config(new DotEnvOptions(envFilePaths: new[] { WhitespacesEnvFileName }, trimValues: false));

        EnvReader.GetStringValue("DB_DATABASE")
            .Should()
            .Be(" laravel  ");
    }

    [Fact]
    public void ConfigShouldLoadEnvWithExistingVarOverwriteOptions()
    {
        Environment.SetEnvironmentVariable("Generic", "Existing");

        DotEnv.Config(new DotEnvOptions(envFilePaths: new[] { GenericEnvFileName }, overwriteExistingVars: false));

        EnvReader.GetStringValue("Generic")
            .Should()
            .Be("Existing");

        DotEnv.Config(new DotEnvOptions(envFilePaths: new[] { GenericEnvFileName }, overwriteExistingVars: true));

        EnvReader.GetStringValue("Generic")
            .Should()
            .Be("Value");
    }

    [Fact]
    public void ConfigShouldLoadDefaultEnvWithProbeOptions()
    {
        var action = new Action(() => DotEnv.Config(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 2, ignoreExceptions: false)));

        action.Should()
            .ThrowExactly<FileNotFoundException>();

        action = () => DotEnv.Config(new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 5, ignoreExceptions: false));

        action.Should()
            .NotThrow();

        EnvReader.GetStringValue("hello")
            .Should()
            .Be("world");
    }

    [Fact]
    public void ConfigShouldLoadEnvWithQuotedValues()
    {
        DotEnv.Config(new DotEnvOptions(envFilePaths: new[] { QuotationsEnvFileName }, trimValues: true));

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
        DotEnv.Config(new DotEnvOptions(envFilePaths: new[] { IncompleteEnvFileName }, trimValues: false));

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

    [Fact]
    public void Should_Parse_Key_With_Colon_Correctly_Using_Read()
    {
        // Arrange
        var envFiles = new[] { "appsettings-oidc-config.env" };

        // Act
        var result = new DotEnvOptions()
             .WithEnvFiles(envFiles)
            .Read();

        // Assert
        result.ContainsKey("OidcAuthentication:ClientId")
            .Should()
            .BeTrue();

        result["OidcAuthentication:ClientId"]
            .Should()
            .Be("your-client-id");

        result.ContainsKey("OidcAuthentication:ClientSecret")
            .Should()
            .BeTrue();

        result["OidcAuthentication:ClientSecret"]
            .Should()
            .Be("your-client-secret");
    }

    [Fact]
    public void Should_Parse_Key_With_Colon_Correctly_And_Load_To_Environment() // this is failing because colones are not supported in environment variable names on Windows. TODO: Evaluate to add a extension or Helper to enable dotenv usage in hostbuilder IConfigurationBuilder?
    {
        // Arrange
        var envFiles = new[] { "appsettings-oidc-config.env" };

        // Act
        new DotEnvOptions()
            .WithEnvFiles(envFiles) 
            .WithProbeForEnv()
            .WithExceptions()
            .Load();

        // Assert
        EnvReader.TryGetStringValue("OidcAuthentication:ClientId", out var value)
            .Should()
            .BeTrue();

        value.Should()
            .Be("your-client-id");

        EnvReader.TryGetStringValue("OidcAuthentication:ClientSecret", out value)
            .Should()
            .BeTrue();

        value.Should()
            .Be("your-client-secret");
    }
}