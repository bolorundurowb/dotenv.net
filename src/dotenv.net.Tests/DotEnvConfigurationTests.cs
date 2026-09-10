using System;
using System.Collections.Generic;
using System.IO;
using dotenv.net.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace dotenv.net.Tests;

[Collection("CurrentDirectory")]
public class DotEnvConfigurationTests
{
    [Fact]
    public void AddDotNetEnv_WithStreams_ShouldExposeValuesThroughIConfiguration()
    {
        using var stream = new MemoryStream("FOO=bar\nBAZ=qux"u8.ToArray());
        var options = new DotEnvOptions().WithEnvStreams(stream);

        var configuration = new ConfigurationBuilder()
            .AddDotNetEnv(options)
            .Build();

        configuration["FOO"].ShouldBe("bar");
        configuration["BAZ"].ShouldBe("qux");
    }

    [Fact]
    public void AddDotNetEnv_ConfigureDelegate_ShouldApplyOptions()
    {
        using var stream = new MemoryStream(" KEY =  value  "u8.ToArray());

        var configuration = new ConfigurationBuilder()
            .AddDotNetEnv(options => options.WithEnvStreams(stream).WithTrimValues())
            .Build();

        configuration["KEY"].ShouldBe("value");
    }

    [Fact]
    public void AddDotNetEnv_Default_ShouldReadEnvFileFromCurrentDirectory()
    {
        using var workspace = new TempEnvDirectory();
        workspace.Write(".env", "DEFAULT_KEY=default-value");

        using (workspace.SetCurrentDirectory())
        {
            var configuration = new ConfigurationBuilder()
                .AddDotNetEnv()
                .Build();

            configuration["DEFAULT_KEY"].ShouldBe("default-value");
        }
    }

    [Fact]
    public void AddDotNetEnv_LaterSource_ShouldOverrideEarlierSource()
    {
        using var envStream = new MemoryStream("KEY=from-env"u8.ToArray());
        var inMemory = new Dictionary<string, string?> { ["KEY"] = "from-memory" };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .AddDotNetEnv(options => options.WithEnvStreams(envStream))
            .Build();

        configuration["KEY"].ShouldBe("from-env");
    }

    [Fact]
    public void AddDotNetEnv_ShouldLookupKeysCaseInsensitively()
    {
        using var stream = new MemoryStream("SomeKey=value"u8.ToArray());

        var configuration = new ConfigurationBuilder()
            .AddDotNetEnv(options => options.WithEnvStreams(stream))
            .Build();

        configuration["somekey"].ShouldBe("value");
        configuration["SOMEKEY"].ShouldBe("value");
    }

    [Fact]
    public void AddDotNetEnv_WithEnvironmentCascade_ShouldLoadLayeredFiles()
    {
        using var workspace = new TempEnvDirectory();
        workspace.Write(".env", "KEY=base");
        workspace.Write(".env.Development", "KEY=development");

        using (workspace.SetCurrentDirectory())
        {
            var configuration = new ConfigurationBuilder()
                .AddDotNetEnv(options => options.WithEnvironmentCascade("Development"))
                .Build();

            configuration["KEY"].ShouldBe("development");
        }
    }

    [Fact]
    public void AddDotNetEnv_ShouldBindToOptionsViaDependencyInjection()
    {
        using var stream = new MemoryStream("Settings:Name=TestApp\nSettings:Timeout=30"u8.ToArray());

        var configuration = new ConfigurationBuilder()
            .AddDotNetEnv(options => options.WithEnvStreams(stream))
            .Build();

        var services = new ServiceCollection();
        services.AddOptions();
        services.Configure<TestSettings>(configuration.GetSection("Settings"));

        using var provider = services.BuildServiceProvider();
        var settings = provider.GetRequiredService<IOptions<TestSettings>>().Value;

        settings.Name.ShouldBe("TestApp");
        settings.Timeout.ShouldBe(30);
    }

    private sealed class TestSettings
    {
        public string? Name { get; set; }
        public int Timeout { get; set; }
    }

    private sealed class TempEnvDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "DotEnvConfig_" + Guid.NewGuid().ToString("N"));

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
}
