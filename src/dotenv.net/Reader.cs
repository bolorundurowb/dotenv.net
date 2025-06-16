using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dotenv.net;

internal static class Reader
{
    internal static ReadOnlySpan<string> ReadFileLines(string envFilePath, bool ignoreExceptions, Encoding? encoding)
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

    internal static ReadOnlySpan<KeyValuePair<string, string>> ExtractEnvKeyValues(ReadOnlySpan<string> rawEnvRows,
        bool trimValues) => rawEnvRows == ReadOnlySpan<string>.Empty
        ? ReadOnlySpan<KeyValuePair<string, string>>.Empty
        : Parser.Parse(rawEnvRows, trimValues);

    internal static Dictionary<string, string> MergeEnvKeyValues(
        IEnumerable<KeyValuePair<string, string>[]> envFileKeyValues, bool overwriteExistingVars)
    {
        var response = new Dictionary<string, string>();

        foreach (var envFileKeyValue in envFileKeyValues)
        foreach (var envKeyValue in envFileKeyValue)
            // if the key does not exist or if a previous env file has the same key, and we are allowed to overwrite it
            if (!response.ContainsKey(envKeyValue.Key) ||
                (response.ContainsKey(envKeyValue.Key) && overwriteExistingVars))
                response[envKeyValue.Key] = envKeyValue.Value;

        return response;
    }

    internal static string GetProbedEnvPath(int levelsToSearch, bool ignoreExceptions)
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
