using System.Collections.Generic;
using System.Linq;

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
                var envKeyValues = Reader.ExtractEnvKeyValues(fileRows, options.TrimValues);
                return envKeyValues.ToArray();
            })
            .ToList();

        return Reader.MergeEnvKeyValues(envFileKeyValues, options.OverwriteExistingVars);
    }

    /// <summary>
    /// Load the values in the provided env files into the environment variables
    /// </summary>
    /// <param name="options">The options required to configure the env loader</param>
    public static void Load(DotEnvOptions? options = null)
    {
        options ??= new DotEnvOptions();
        var envVars = Read(options);
        Writer.WriteToEnv(envVars, options.OverwriteExistingVars);
    }
}
