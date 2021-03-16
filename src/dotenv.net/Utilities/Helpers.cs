using System;
using System.Collections.Generic;
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

        public static IDictionary<string, string> ReadAndReturn(IEnumerable<string> envFilePaths,
            bool ignoreExceptions, Encoding encoding, bool trimValues)
        {
            var response = new Dictionary<string, string>();

            foreach (var envFilePath in envFilePaths)
            {
                var envRows = ReadAndParse(envFilePath, ignoreExceptions, encoding, trimValues);
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

        public static void ReadAndWrite(IEnumerable<string> envFilePaths,
            bool ignoreExceptions, Encoding encoding, bool trimValues, bool overwriteExisting)
        {
            foreach (var envFilePath in envFilePaths)
            {
                var envRows = ReadAndParse(envFilePath, ignoreExceptions, encoding, trimValues);
                foreach (var envRow in envRows)
                {
                    if (overwriteExisting)
                    {
                        Environment.SetEnvironmentVariable(envRow.Key, envRow.Value);
                    }
                    else if (!EnvReader.HasValue(envRow.Key))
                    {
                        Environment.SetEnvironmentVariable(envRow.Key, envRow.Value);
                    }
                }
            }
        }
    }
}