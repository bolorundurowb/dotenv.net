using System.Collections.Generic;
using System.Linq;

namespace dotenv.net;

/// <summary>
/// Provides static methods to configure, read, and load environment variables from .env files.
/// </summary>
public static class DotEnv
{
    /// <summary>
    /// Initialises the fluent configuration API.
    /// </summary>
    /// <returns>A new instance of <see cref="DotEnvOptions" />.</returns>
    public static DotEnvOptions Fluent() => new();

    /// <summary>
    /// Reads the values from the provided env files based on the specified options.
    /// </summary>
    /// <param name="options">The options required to configure the env loader. If null, default options are used.</param>
    /// <returns>A dictionary containing the key-value pairs read from the env files.</returns>
    public static IDictionary<string, string> Read(DotEnvOptions? options = null)
    {
        options ??= new DotEnvOptions();
        var envFilePaths = options.ProbeForEnv
            ? Reader.GetProbedEnvPath(options.ProbeLevelsToSearch!.Value, options.IgnoreExceptions)
            : options.EnvFilePaths;
        var envFileKeyValues = envFilePaths
            .Select(envFilePath =>
            {
                var fileRows = Reader.ReadFileLines(envFilePath, options.IgnoreExceptions, options.Encoding);
                var envKeyValues =
                    Reader.ExtractEnvKeyValues(fileRows, options.TrimValues, options.SupportExportSyntax);
                return envKeyValues.ToArray();
            })
            .ToList();

        return Reader.MergeEnvKeyValues(envFileKeyValues, options.OverwriteExistingVars);
    }

    /// <summary>
    /// Loads the values from the provided env files into the system environment variables.
    /// </summary>
    /// <param name="options">The options required to configure the env loader. If null, default options are used.</param>
    public static void Load(DotEnvOptions? options = null)
    {
        options ??= new DotEnvOptions();
        var envVars = Read(options);
        Writer.WriteToEnv(envVars, options.OverwriteExistingVars);
    }
}