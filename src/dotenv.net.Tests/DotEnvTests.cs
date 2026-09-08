using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class DotEnvTests
{
    [Fact]
    public void Read_WithNullOptions_ShouldUseDefaultOptions()
    {
        var values = DotEnv.Read(null);
        values.ShouldNotBeNull();
    }

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

    [Fact]
    public void Read_WithVariableExpansion_FromStreams_ShouldResolveVariablesAcrossStreams()
    {
        using var first = new MemoryStream("HOST=example.com"u8.ToArray());
        using var second = new MemoryStream("URL=https://${HOST}/api"u8.ToArray());

        var options = new DotEnvOptions(supportVariableExpansion: true).WithEnvStreams(first, second);
        var values = DotEnv.Read(options);

        values["HOST"].ShouldBe("example.com");
        values["URL"].ShouldBe("https://example.com/api");
    }

    [Fact]
    public void Read_WithVariableExpansion_FromMultipleStreams_WithoutOverwrite_ShouldPreserveFirstVariableExpansion()
    {
        using var first = new MemoryStream("HOST=primary.com"u8.ToArray());
        using var second = new MemoryStream("HOST=secondary.com\nURL=https://${HOST}"u8.ToArray());

        var options = new DotEnvOptions(overwriteExistingVars: false, supportVariableExpansion: true).WithEnvStreams(first, second);
        var values = DotEnv.Read(options);

        values["HOST"].ShouldBe("primary.com");
        values["URL"].ShouldBe("https://primary.com");
    }

    [Fact]
    public void Read_DefaultOptions_FromStream_ShouldNotExpandVariables()
    {
        using var stream = new MemoryStream("A=foo\nB=${A}"u8.ToArray());
        var options = new DotEnvOptions().WithEnvStreams(stream);
        var values = DotEnv.Read(options);

        values["A"].ShouldBe("foo");
        values["B"].ShouldBe("${A}");
    }

    [Fact]
    public void Read_WithoutVariableExpansion_FromStream_ShouldNotExpandVariables()
    {
        using var stream = new MemoryStream("A=foo\nB=${A}"u8.ToArray());
        var options = new DotEnvOptions().WithoutVariableExpansion().WithEnvStreams(stream);
        var values = DotEnv.Read(options);

        values["A"].ShouldBe("foo");
        values["B"].ShouldBe("${A}");
    }

    [Fact]
    public void Load_WithVariableExpansion_ShouldWriteExpandedValuesToEnvironment()
    {
        var key = "DOTENV_TEST_EXPAND_" + Guid.NewGuid().ToString("N");
        var refKey = "DOTENV_TEST_REF_" + Guid.NewGuid().ToString("N");

        try
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes($"{key}=root\n{refKey}=${{{key}}}/child"));
            DotEnv.Fluent()
                .WithEnvStreams(stream)
                .WithSupportVariableExpansion()
                .Load();

            Environment.GetEnvironmentVariable(key).ShouldBe("root");
            Environment.GetEnvironmentVariable(refKey).ShouldBe("root/child");
        }
        finally
        {
            Environment.SetEnvironmentVariable(key, null);
            Environment.SetEnvironmentVariable(refKey, null);
        }
    }

    [Fact]
    public void Read_WithEnvironmentCascade_ShouldOverrideInDotenvFlowOrder()
    {
        using var workspace = new TempEnvDirectory();
        workspace.Write(".env", "KEY=base\nONLY_BASE=yes");
        workspace.Write(".env.local", "KEY=local\nONLY_LOCAL=yes");
        workspace.Write(".env.Development", "KEY=development\nONLY_DEV=yes");
        workspace.Write(".env.Development.local", "KEY=development-local\nONLY_DEV_LOCAL=yes");

        using (workspace.SetCurrentDirectory())
        {
            var values = DotEnv.Fluent()
                .WithEnvironmentCascade("Development")
                .Read();

            values["KEY"].ShouldBe("development-local");
            values["ONLY_BASE"].ShouldBe("yes");
            values["ONLY_LOCAL"].ShouldBe("yes");
            values["ONLY_DEV"].ShouldBe("yes");
            values["ONLY_DEV_LOCAL"].ShouldBe("yes");
        }
    }

    [Fact]
    public void Read_WithEnvironmentCascade_WhenNoEnvironment_ShouldLoadBaseAndLocalOnly()
    {
        using var workspace = new TempEnvDirectory();
        workspace.Write(".env", "KEY=base");
        workspace.Write(".env.local", "KEY=local");
        workspace.Write(".env.Development", "KEY=development");

        using (new EnvironmentVariableScope(
                   (EnvFileCascade.AspNetCoreEnvironmentVariable, null),
                   (EnvFileCascade.DotNetEnvironmentVariable, null),
                   (EnvFileCascade.DotEnvEnvironmentVariable, null)))
        using (workspace.SetCurrentDirectory())
        {
            var values = DotEnv.Fluent().WithEnvironmentCascade().Read();
            values["KEY"].ShouldBe("local");
        }
    }

    [Fact]
    public void Read_WithoutEnvironmentCascade_ShouldNotLoadLayeredFiles()
    {
        using var workspace = new TempEnvDirectory();
        workspace.Write(".env", "KEY=base");
        workspace.Write(".env.local", "KEY=local");

        using (workspace.SetCurrentDirectory())
        {
            var values = DotEnv.Read();
            values["KEY"].ShouldBe("base");
        }
    }

    [Fact]
    public void Read_WithEnvironmentCascade_MissingFiles_ShouldSkipWhenExceptionsIgnored()
    {
        using var workspace = new TempEnvDirectory();
        workspace.Write(".env", "KEY=base");

        using (workspace.SetCurrentDirectory())
        {
            var values = DotEnv.Fluent()
                .WithoutExceptions()
                .WithEnvironmentCascade("Production")
                .Read();

            values["KEY"].ShouldBe("base");
        }
    }

    [Fact]
    public void Read_WithEnvironmentCascade_WhenNoFilesExistAndExceptionsEnabled_ShouldThrow()
    {
        using var workspace = new TempEnvDirectory();

        using (workspace.SetCurrentDirectory())
        {
            Should.Throw<FileNotFoundException>(() =>
                DotEnv.Fluent().WithExceptions().WithEnvironmentCascade("Development").Read());
        }
    }

    [Fact]
    public void Read_WithEnvironmentCascadeAndProbeForEnv_ShouldLoadFromProbedDirectory()
    {
        using var workspace = new TempEnvDirectory();
        var nested = Directory.CreateDirectory(Path.Combine(workspace.Path, "a", "b", "c"));
        workspace.Write(".env", "KEY=base");
        workspace.Write(".env.Staging", "KEY=staging");

        var directory = Reader.GetProbedEnvDirectory(3, ignoreExceptions: false,
            EnvFileCascade.GetCandidateFileNames("Staging"), nested.FullName);
        directory.ShouldBe(workspace.Path);

        var paths = EnvFileCascade.ResolveExistingFilePaths(directory!, "Staging", ignoreExceptions: false);
        paths.Count.ShouldBe(2);
    }

    [Fact]
    public void Read_WithEnvironmentCascadeAndProbeForEnv_ShouldReadLayeredFilesFromProbedDirectory()
    {
        var environmentName = "T" + Guid.NewGuid().ToString("N")[..8];
        var envFile = Path.Combine(AppContext.BaseDirectory, $".env.{environmentName}");
        var localFile = Path.Combine(AppContext.BaseDirectory, $".env.{environmentName}.local");

        File.WriteAllText(envFile, "KEY=probed");
        File.WriteAllText(localFile, "KEY=probed-local");

        try
        {
            var values = DotEnv.Fluent()
                .WithProbeForEnv(0)
                .WithEnvironmentCascade(environmentName)
                .Read();

            values["KEY"].ShouldBe("probed-local");
        }
        finally
        {
            File.Delete(envFile);
            File.Delete(localFile);
        }
    }

    [Fact]
    public void Read_WithEnvironmentCascadeAndProbeForEnv_WhenNotFoundAndExceptionsIgnored_ShouldReturnEmpty()
    {
        var environmentName = "M" + Guid.NewGuid().ToString("N")[..8];
        var values = DotEnv.Fluent()
            .WithoutExceptions()
            .WithProbeForEnv(0)
            .WithEnvironmentCascade(environmentName)
            .Read();

        values.ShouldBeEmpty();
    }

    [Fact]
    public void Read_WithEnvironmentCascadeAndProbeForEnv_WhenNotFoundAndExceptionsEnabled_ShouldThrow()
    {
        var environmentName = "M" + Guid.NewGuid().ToString("N")[..8];
        Should.Throw<FileNotFoundException>(() =>
            DotEnv.Fluent()
                .WithExceptions()
                .WithProbeForEnv(0)
                .WithEnvironmentCascade(environmentName)
                .Read());
    }

    [Fact]
    public void Read_WithEnvironmentCascade_UnsafeEnvironmentName_ShouldThrow()
    {
        Should.Throw<ArgumentException>(() =>
            DotEnv.Fluent().WithEnvironmentCascade("../Production").Read());
    }

    [Fact]
    public void Read_WithEnvironmentCascade_ShouldUseAspNetCoreEnvironmentVariable()
    {
        using var workspace = new TempEnvDirectory();
        var environmentName = "E" + Guid.NewGuid().ToString("N")[..8];
        workspace.Write(".env", "KEY=base");
        workspace.Write($".env.{environmentName}", "KEY=from-aspnet");

        using (new EnvironmentVariableScope(
                   (EnvFileCascade.AspNetCoreEnvironmentVariable, environmentName),
                   (EnvFileCascade.DotNetEnvironmentVariable, null),
                   (EnvFileCascade.DotEnvEnvironmentVariable, null)))
        using (workspace.SetCurrentDirectory())
        {
            var values = DotEnv.Fluent().WithEnvironmentCascade().Read();
            values["KEY"].ShouldBe("from-aspnet");
        }
    }

    [Fact]
    public void Read_WithEnvironmentCascade_WithoutOverwrite_ShouldPreferEarlierFiles()
    {
        using var workspace = new TempEnvDirectory();
        workspace.Write(".env", "KEY=base");
        workspace.Write(".env.local", "KEY=local");

        using (workspace.SetCurrentDirectory())
        {
            var values = DotEnv.Fluent()
                .WithoutOverwriteExistingVars()
                .WithEnvironmentCascade()
                .Read();

            values["KEY"].ShouldBe("base");
        }
    }

    [Fact]
    public void Load_WithEnvironmentCascade_ShouldWriteHighestPrecedenceValue()
    {
        using var workspace = new TempEnvDirectory();
        var key = "DOTENV_CASCADE_" + Guid.NewGuid().ToString("N");
        workspace.Write(".env", $"{key}=base");
        workspace.Write(".env.local", $"{key}=local");

        try
        {
            using (workspace.SetCurrentDirectory())
            {
                DotEnv.Fluent().WithEnvironmentCascade().Load();
            }

            Environment.GetEnvironmentVariable(key).ShouldBe("local");
        }
        finally
        {
            Environment.SetEnvironmentVariable(key, null);
        }
    }

    private sealed class TempEnvDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "DotEnvCascade_" + Guid.NewGuid().ToString("N"));

        public TempEnvDirectory() => Directory.CreateDirectory(Path);

        public void Write(string fileName, string contents) =>
            File.WriteAllText(System.IO.Path.Combine(Path, fileName), contents);

        public CurrentDirectoryScope SetCurrentDirectory() => new(Path);

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);
        }
    }

    private sealed class CurrentDirectoryScope : IDisposable
    {
        private readonly string _original = Directory.GetCurrentDirectory();

        public CurrentDirectoryScope(string directory) => Directory.SetCurrentDirectory(directory);

        public void Dispose() => Directory.SetCurrentDirectory(_original);
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly (string Key, string? Value)[] _original;

        public EnvironmentVariableScope(params (string Key, string? Value)[] values)
        {
            _original = new (string, string?)[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var key = values[i].Key;
                _original[i] = (key, Environment.GetEnvironmentVariable(key));
                Environment.SetEnvironmentVariable(key, values[i].Value);
            }
        }

        public void Dispose()
        {
            foreach (var (key, value) in _original)
                Environment.SetEnvironmentVariable(key, value);
        }
    }
}