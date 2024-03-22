using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dotenv.net.Utilities
{
    internal static class Helpers
    {
 private static ReadOnlySpan<KeyValuePair<string, string>> ReadAndParse(string envFilePath,
            bool ignoreExceptions, Encoding encoding, bool trimValues)
        {
            var rawEnvRows = Reader.Read(envFilePath, ignoreExceptions, encoding);

            if (rawEnvRows == ReadOnlySpan<string>.Empty)
            {
                return ReadOnlySpan<KeyValuePair<string, string>>.Empty;
            }

            return ParseForMultilines(rawEnvRows, trimValues);
        }

        /// <summary>
        /// Parses potential multi-line values, surrounded by quotation marks.
        /// </summary>
        /// <param name="rawEnvRows">Unedited rows from the env file.</param>
        /// <param name="trimValues">Whether a line should be trimmed or not.</param>
        /// <returns>KeyValuePairs, whose multi-line values were properly merged into a single value each.</returns>
        private static ReadOnlySpan<KeyValuePair<string, string>> ParseForMultilines(ReadOnlySpan<string> rawEnvRows, bool trimValues)
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>();

            for (var i = 0; i < rawEnvRows.Length; i++)
            {
                var rawEnvRow = rawEnvRows[i];

                if (rawEnvRow.Contains("=\""))
                {
                    var key = rawEnvRow.Substring(0, rawEnvRow.IndexOf("=\"", StringComparison.Ordinal));
                    var valueStringBuilder = new StringBuilder();
                    valueStringBuilder.Append(rawEnvRow.Substring(rawEnvRow.IndexOf("=\"", StringComparison.Ordinal) + 2));

                    while (!rawEnvRow.EndsWith("\""))
                    {
                        i++;
                        if (i >= rawEnvRows.Length)
                        {
                            break;
                        }
                        rawEnvRow = rawEnvRows[i];
                        valueStringBuilder.Append(rawEnvRow);
                    }
                    //Remove last "
                    valueStringBuilder.Remove(valueStringBuilder.Length - 1, 1);

                    var value = valueStringBuilder.ToString();
                    if (trimValues)
                    {
                        value = value.Trim();
                    }

                    keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
                }
                else
                {
                    // Regular key-value pair
                    var keyValue = rawEnvRow.Split(new[] { '=' }, 2);

                    var key = keyValue[0];
                    var value = keyValue[1];

                    if (trimValues)
                    {
                        value = value.Trim();
                    }

                    keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            return keyValuePairs.ToArray();
        }

        internal static IDictionary<string, string> ReadAndReturn(DotEnvOptions options)
        {
            var response = new Dictionary<string, string>();
            var envFilePaths = options.ProbeForEnv
                ? new[] {GetProbedEnvPath(options.ProbeLevelsToSearch, options.IgnoreExceptions)}
                : options.EnvFilePaths;

            foreach (var envFilePath in envFilePaths)
            {
                var envRows = ReadAndParse(envFilePath, options.IgnoreExceptions, options.Encoding,
                    options.TrimValues);

                foreach (var envRow in envRows)
                {
                    if (response.ContainsKey(envRow.Key))
                    {
                        response[envRow.Key] = envRow.Value;
                    }
                    else
                    {
                        response.Add(envRow.Key, envRow.Value);
                    }
                }
            }

            return response;
        }

        internal static void ReadAndWrite(DotEnvOptions options)
        {
            var envVars = ReadAndReturn(options);

            foreach (var envVar in envVars)
            {
                if (options.OverwriteExistingVars)
                {
                    Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
                }
                else if (!EnvReader.HasValue(envVar.Key))
                {
                    Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
                }
            }
        }

        private static string GetProbedEnvPath(int levelsToSearch, bool ignoreExceptions)
        {
            var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
            var count = levelsToSearch;
            var foundEnvPath = SearchPaths();

            if (string.IsNullOrEmpty(foundEnvPath) && !ignoreExceptions)
            {
                throw new FileNotFoundException(
                    $"Failed to find a file matching the '{DotEnvOptions.DefaultEnvFileName}' search pattern." +
                    $"{Environment.NewLine}Current Directory: {currentDirectory}" +
                    $"{Environment.NewLine}Levels Searched: {levelsToSearch}");
            }

            return foundEnvPath;


            string SearchPaths()
            {
                for (;
                    currentDirectory != null && count > 0;
                    count--, currentDirectory = currentDirectory.Parent)
                {
                    foreach (var file in currentDirectory.GetFiles(DotEnvOptions.DefaultEnvFileName,
                        SearchOption.TopDirectoryOnly))
                    {
                        return file.FullName;
                    }
                }

                return null;
            }
        }
    }
}