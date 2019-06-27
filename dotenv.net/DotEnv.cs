using System;
using System.IO;
using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;

namespace dotenv.net
{
    public class DotEnv
    {
        private static DotEnv _instance;

        private static DotEnv Instance => _instance ?? (_instance = new DotEnv());

        private void ConfigRunner(bool throwOnError, string filePath, Encoding encoding, bool trimValues)
        {
            // if configured to throw errors then throw otherwise return
            if (!File.Exists(filePath))
            {
                if (throwOnError)
                {
                    throw new FileNotFoundException($"An environment file with path \"{filePath}\" does not exist.");
                }

                return;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            // read all lines from the env file
            var dotEnvContents = File.ReadAllText(filePath, encoding);

            // split the long string into an array of rows
            var dotEnvRows = dotEnvContents.Split(new[] {"\n", "\r\n", Environment.NewLine},
                StringSplitOptions.RemoveEmptyEntries);

            // loop through rows, split into key and value then add to environment
            foreach (var dotEnvRow in dotEnvRows)
            {
                var row = trimValues ? dotEnvRow.Trim() : dotEnvRow;

                // determine if row is empty
                if (string.IsNullOrEmpty(row))
                    continue;

                // determine if row is comment
                if (row.StartsWith("#"))
                    continue;

                var index = row.IndexOf('#');

                // if there is no key, skip
                if (index <= 0)
                    continue;

                var key = dotEnvRow.Substring(0, index).Trim();
                var value = dotEnvRow.Substring(index + 1, dotEnvRow.Length - (index + 1)).Trim();

                Environment.SetEnvironmentVariable(key, value);
            }
        }

        /// <summary>
        /// Configure the environment variables from a .env file
        /// </summary>
        /// <param name="throwOnError">A value stating whether the application should throw an exception on unexpected data</param>
        /// <param name="filePath">An optional env file path, if not provided it defaults to the one in the same folder as the output exe or dll</param>
        /// <param name="encoding">The encoding with which the env file was created, It defaults to the platforms default</param>
        /// <param name="trimValues">This determines whether not whitespace is trimmed from the values. It defaults to true</param>
        /// <exception cref="FileNotFoundException">Thrown if the env file doesn't exist</exception>
        public static void Config(bool throwOnError = true, string filePath = ".env", Encoding encoding = null,
            bool trimValues = true)
        {
            Instance.ConfigRunner(throwOnError, filePath, encoding, trimValues);
        }

        /// <summary>
        /// Configure the environment variables from a .env file
        /// </summary>
        /// <param name="options">Options on how to load the env file</param>
        public static void Config(DotEnvOptions options)
        {
            Instance.ConfigRunner(options.ThrowOnError, options.EnvFile, options.Encoding, options.TrimValues);
        }
    }
}