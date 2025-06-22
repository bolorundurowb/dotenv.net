using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    internal static IEnumerable<string> GetProbedEnvPath(int levelsToSearch, bool ignoreExceptions)
    {
        var pathsSearched = new List<string>();
        var count = levelsToSearch;
        var foundEnvPath = SearchPaths();

        if (string.IsNullOrEmpty(foundEnvPath) && !ignoreExceptions)
            throw new FileNotFoundException(
                $"Could not find '{DotEnvOptions.DefaultEnvFileName}' after searching {levelsToSearch} directory level(s) upwards.{Environment.NewLine}Searched paths:{Environment.NewLine}{string.Join(Environment.NewLine, pathsSearched)}");

        return foundEnvPath == null ? [] : [foundEnvPath];

        string? SearchPaths()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            for (var i = 0; i <= count; i++)
            {
                if (directory == null)
                    break;

                pathsSearched.Add(directory.FullName);

                foreach (var fileInfo in directory.EnumerateFiles(DotEnvOptions.DefaultEnvFileName,
                             SearchOption.TopDirectoryOnly))
                    return fileInfo.FullName;

                directory = directory.Parent;
            }

            return null;
        }
    }
}
