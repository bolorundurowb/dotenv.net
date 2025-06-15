using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using dotenv.net.Utilities;

namespace dotenv.net;

internal static class Reader
{
    internal static ReadOnlySpan<string> Read(string envFilePath, bool ignoreExceptions, Encoding? encoding)
    {
        var defaultResponse = ReadOnlySpan<string>.Empty;

        // if configured to throw errors then throw otherwise return
        if (string.IsNullOrWhiteSpace(envFilePath))
        {
            if (ignoreExceptions)
                return defaultResponse;

            throw new ArgumentException("The file path cannot be null, empty or whitespace.", nameof(envFilePath));
        }

        // if configured to throw errors then throw otherwise return
        if (!File.Exists(envFilePath))
        {
            if (ignoreExceptions)
                return defaultResponse;

            throw new FileNotFoundException($"A file with provided path \"{envFilePath}\" does not exist.");
        }

        // default to UTF8 if encoding is not provided
        encoding ??= Encoding.UTF8;

        // read all lines from the env file
        return new ReadOnlySpan<string>(File.ReadAllLines(envFilePath, encoding));
    }

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
