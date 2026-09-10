using System;
using Microsoft.Extensions.Configuration;

namespace dotenv.net.Configuration;

/// <summary>
/// A <see cref="ConfigurationProvider"/> that loads key-value pairs from .env files using dotenv.net.
/// </summary>
public sealed class DotEnvConfigurationProvider : ConfigurationProvider
{
    private readonly DotEnvOptions _options;

    /// <summary>
    /// Initialises a new instance of the <see cref="DotEnvConfigurationProvider"/> class.
    /// </summary>
    /// <param name="options">The options used to configure the .env loading behaviour.</param>
    /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
    public DotEnvConfigurationProvider(DotEnvOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Reads the .env files and loads the parsed key-value pairs into the configuration data.
    /// </summary>
    public override void Load()
    {
        var values = DotEnv.Read(_options);
        foreach (var keyValue in values)
            Data[keyValue.Key] = keyValue.Value;
    }
}
