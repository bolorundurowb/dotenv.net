using System;
using System.IO;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

public class EnvFileCascadeTests
{
    [Fact]
    public void GetCandidateFileNames_WithoutEnvironment_ShouldReturnBaseAndLocalOnly()
    {
        EnvFileCascade.GetCandidateFileNames(null).ShouldBe([".env", ".env.local"]);
        EnvFileCascade.GetCandidateFileNames(" ").ShouldBe([".env", ".env.local"]);
    }

    [Fact]
    public void GetCandidateFileNames_WithEnvironment_ShouldAppendEnvironmentFiles()
    {
        EnvFileCascade.GetCandidateFileNames("Development")
            .ShouldBe([".env", ".env.local", ".env.Development", ".env.Development.local"]);
    }

    [Theory]
    [InlineData("../Production")]
    [InlineData("Prod/uction")]
    [InlineData(@"Prod\uction")]
    public void GetCandidateFileNames_WithUnsafeEnvironmentName_ShouldThrow(string environmentName)
    {
        Should.Throw<ArgumentException>(() => EnvFileCascade.GetCandidateFileNames(environmentName))
            .ParamName.ShouldBe("environmentName");
    }

    [Fact]
    public void ResolveEnvironmentName_ExplicitName_ShouldWinOverEnvironmentVariables()
    {
        using var _ = new EnvironmentVariableScope(
            (EnvFileCascade.AspNetCoreEnvironmentVariable, "AspNet"),
            (EnvFileCascade.DotNetEnvironmentVariable, "DotNet"),
            (EnvFileCascade.DotEnvEnvironmentVariable, "DotEnv"));

        EnvFileCascade.ResolveEnvironmentName(" Explicit ").ShouldBe("Explicit");
    }

    [Fact]
    public void ResolveEnvironmentName_ShouldPreferAspNetCoreThenDotNetThenDotEnv()
    {
        using (new EnvironmentVariableScope(
                   (EnvFileCascade.AspNetCoreEnvironmentVariable, "AspNet"),
                   (EnvFileCascade.DotNetEnvironmentVariable, "DotNet"),
                   (EnvFileCascade.DotEnvEnvironmentVariable, "DotEnv")))
        {
            EnvFileCascade.ResolveEnvironmentName(null).ShouldBe("AspNet");
        }

        using (new EnvironmentVariableScope(
                   (EnvFileCascade.AspNetCoreEnvironmentVariable, null),
                   (EnvFileCascade.DotNetEnvironmentVariable, "DotNet"),
                   (EnvFileCascade.DotEnvEnvironmentVariable, "DotEnv")))
        {
            EnvFileCascade.ResolveEnvironmentName(null).ShouldBe("DotNet");
        }

        using (new EnvironmentVariableScope(
                   (EnvFileCascade.AspNetCoreEnvironmentVariable, null),
                   (EnvFileCascade.DotNetEnvironmentVariable, null),
                   (EnvFileCascade.DotEnvEnvironmentVariable, "DotEnv")))
        {
            EnvFileCascade.ResolveEnvironmentName(null).ShouldBe("DotEnv");
        }

        using (new EnvironmentVariableScope(
                   (EnvFileCascade.AspNetCoreEnvironmentVariable, null),
                   (EnvFileCascade.DotNetEnvironmentVariable, null),
                   (EnvFileCascade.DotEnvEnvironmentVariable, null)))
        {
            EnvFileCascade.ResolveEnvironmentName(null).ShouldBeNull();
        }
    }

    [Fact]
    public void ResolveExistingFilePaths_ShouldSkipMissingFilesAndPreserveOrder()
    {
        var directory = Path.Combine(Path.GetTempPath(), "DotEnvCascade_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            File.WriteAllText(Path.Combine(directory, ".env"), "A=base");
            File.WriteAllText(Path.Combine(directory, ".env.Development"), "A=dev");

            var paths = EnvFileCascade.ResolveExistingFilePaths(directory, "Development", ignoreExceptions: true);

            paths.ShouldBe([
                Path.Combine(directory, ".env"),
                Path.Combine(directory, ".env.Development")
            ]);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void ResolveExistingFilePaths_WhenNoFilesExistAndExceptionsEnabled_ShouldThrow()
    {
        var directory = Path.Combine(Path.GetTempPath(), "DotEnvCascade_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            Should.Throw<FileNotFoundException>(() =>
                    EnvFileCascade.ResolveExistingFilePaths(directory, "Development", ignoreExceptions: false))
                .Message.ShouldContain(".env");
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void ResolveExistingFilePaths_WhenNoFilesExistAndExceptionsIgnored_ShouldReturnEmpty()
    {
        var directory = Path.Combine(Path.GetTempPath(), "DotEnvCascade_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        try
        {
            EnvFileCascade.ResolveExistingFilePaths(directory, null, ignoreExceptions: true)
                .ShouldBeEmpty();
        }
        finally
        {
            Directory.Delete(directory, true);
        }
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
