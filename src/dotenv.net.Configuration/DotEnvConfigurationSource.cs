using System;
using Microsoft.Extensions.Configuration;

namespace dotenv.net.Configuration;

/// <summary>
/// An <see cref="IConfigurationSource"/> that produces a <see cref="DotEnvConfigurationProvider"/>
/// for loading variables from .env files.
/// </summary>
public sealed class DotEnvConfigurationSource : IConfigurationSource
{
    private readonly DotEnvOptions _options;

    /// <summary>
    /// Initialises a new instance of the <see cref="DotEnvConfigurationSource"/> class.
    /// </summary>
    /// <param name="options">The options used to configure the .env loading behaviour.</param>
    /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
    public DotEnvConfigurationSource(DotEnvOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DotEnvConfigurationProvider(_options);
    }
}
