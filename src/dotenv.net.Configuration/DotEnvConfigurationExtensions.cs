using System;
using Microsoft.Extensions.Configuration;

namespace dotenv.net.Configuration;

/// <summary>
/// Extension methods for adding dotenv.net as a configuration source.
/// </summary>
public static class DotEnvConfigurationExtensions
{
    /// <summary>
    /// Adds the .env files as a configuration source using the default options.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add the source to.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    public static IConfigurationBuilder AddDotNetEnv(this IConfigurationBuilder builder)
    {
        return AddDotNetEnv(builder, new DotEnvOptions());
    }

    /// <summary>
    /// Adds the .env files as a configuration source using the provided options.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add the source to.</param>
    /// <param name="options">The options used to configure the .env loading behaviour.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder or options is null.</exception>
    public static IConfigurationBuilder AddDotNetEnv(this IConfigurationBuilder builder, DotEnvOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        return builder.Add(new DotEnvConfigurationSource(options));
    }

    /// <summary>
    /// Adds the .env files as a configuration source using the options configured by the delegate.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add the source to.</param>
    /// <param name="configure">A delegate used to configure the <see cref="DotEnvOptions"/>.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder or configure is null.</exception>
    public static IConfigurationBuilder AddDotNetEnv(this IConfigurationBuilder builder,
        Action<DotEnvOptions> configure)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        var options = new DotEnvOptions();
        configure(options);
        return AddDotNetEnv(builder, options);
    }
}
