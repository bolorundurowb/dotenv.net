using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dotenv.net.Utilities;

internal static class Helpers
{
    private static ReadOnlySpan<KeyValuePair<string, string>> ReadAndParse(string envFilePath,
        bool ignoreExceptions, Encoding encoding, bool trimValues)
    {
        var rawEnvRows = Reader.Read(envFilePath, ignoreExceptions, encoding);

        return rawEnvRows == ReadOnlySpan<string>.Empty
            ? ReadOnlySpan<KeyValuePair<string, string>>.Empty
            : Parser.Parse(rawEnvRows, trimValues);
    }

    internal static IDictionary<string, string> ReadAndReturn(DotEnvOptions options)
    {
        var response = new Dictionary<string, string>();
        var envFilePaths = options.ProbeForEnv
            ? [GetProbedEnvPath(options.ProbeLevelsToSearch, options.IgnoreExceptions)]
            : options.EnvFilePaths;

        foreach (var envFilePath in envFilePaths)
        {
            var envRows = ReadAndParse(envFilePath, options.IgnoreExceptions, options.Encoding,
                options.TrimValues);

            foreach (var envRow in envRows)
                response[envRow.Key] = envRow.Value;
        }

        return response;
    }

    internal static void ReadAndWrite(DotEnvOptions options)
    {
        var envVars = ReadAndReturn(options);

        foreach (var envVar in envVars)
            if (options.OverwriteExistingVars || !EnvReader.HasValue(envVar.Key))
                Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
    }

    private static string GetProbedEnvPath(int levelsToSearch, bool ignoreExceptions)
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        var count = levelsToSearch;
        var foundEnvPath = SearchPaths();

        if (string.IsNullOrEmpty(foundEnvPath) && !ignoreExceptions)
            throw new FileNotFoundException(
                $"Failed to find a file matching the '{DotEnvOptions.DefaultEnvFileName}' search pattern." +
                $"{Environment.NewLine}Current Directory: {currentDirectory}" +
                $"{Environment.NewLine}Levels Searched: {levelsToSearch}");

        return foundEnvPath;


        string? SearchPaths()
        {
            for (;
                 currentDirectory != null && count > 0;
                 count--, currentDirectory = currentDirectory.Parent
                )
                foreach (var file in currentDirectory.GetFiles(
                             DotEnvOptions.DefaultEnvFileName, SearchOption.TopDirectoryOnly)
                        )
                    return file.FullName;

            return null;
        }
    }
}
