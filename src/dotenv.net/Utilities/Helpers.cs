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

        public static IDictionary<string, string> ReadAndReturn(string envFilePath,
            bool ignoreExceptions, Encoding encoding, bool trimValues)
        {
            var response = new Dictionary<string, string>();
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

            return response;
        }

        public static void ReadAndWrite(string envFilePath,
            bool ignoreExceptions, Encoding encoding, bool trimValues)
        {
            var envRows = ReadAndParse(envFilePath, ignoreExceptions, encoding, trimValues);
            foreach (var envRow in envRows)
            {
                Environment.SetEnvironmentVariable(envRow.Key, envRow.Value);
            }
        }
    }
}