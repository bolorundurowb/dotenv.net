using System.Collections.Generic;
using dotenv.net.Utilities;

namespace dotenv.net;

public static class DotEnv
{
    /// <summary>
    /// Initialize the fluent configuration API
    /// </summary>
    public static DotEnvOptions Fluent() => new();

    /// <summary>
    /// Read and return the values in the provided env files
    /// </summary>
    /// <param name="options">The options required to configure the env loader</param>
    /// <returns>The key value pairs read from the env files</returns>
    public static IDictionary<string, string> Read(DotEnvOptions? options = null) =>
        Helpers.ReadAndReturn(options ?? new DotEnvOptions());

    /// <summary>
    /// Load the values in the provided env files into the environment variables
    /// </summary>
    /// <param name="options">The options required to configure the env loader</param>
    public static void Load(DotEnvOptions? options = null) => Helpers.ReadAndWrite(options ?? new DotEnvOptions());
}
