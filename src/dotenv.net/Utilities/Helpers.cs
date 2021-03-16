using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dotenv.net.Utilities
{
    internal static class Helpers
    {
        public static ReadOnlySpan<KeyValuePair<string, string>> ReadAndParse(string envFilePath,
            bool ignoreExceptions, Encoding encoding, bool trimValues)
        {
            var rawEnvRows = Reader.Read(envFilePath, ignoreExceptions, encoding);

            if (rawEnvRows == ReadOnlySpan<string>.Empty)
            {
                return ReadOnlySpan<KeyValuePair<string, string>>.Empty;
            }

            return Parser.Parse(rawEnvRows, trimValues);
        }

        public static IDictionary<string, string> ReadAndReturn(DotEnvOptions options)
        {
            var response = new Dictionary<string, string>();

            if (options.ShouldProbeForEnv)
            {
                options.EnvFilePaths = new[] {GetProbedEnvPath(options.ProbeDirectoryDepth)};
            }

            foreach (var envFilePath in options.EnvFilePaths)
            {
                var envRows = ReadAndParse(envFilePath, options.ShouldIgnoreExceptions, options.Encoding,
                    options.ShouldTrimValues);
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

        public static void ReadAndWrite(DotEnvOptions options)
        {
            var envVars = ReadAndReturn(options);

            foreach (var envVar in envVars)
            {
                if (options.ShouldOverwriteExistingVars)
                {
                    Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
                }
                else if (!EnvReader.HasValue(envVar.Key))
                {
                    Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
                }
            }
        }

        private static string GetProbedEnvPath(int levelsToSearch)
        {
            var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

            for (;
                currentDirectory != null && levelsToSearch > 0;
                levelsToSearch--, currentDirectory = currentDirectory.Parent)
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