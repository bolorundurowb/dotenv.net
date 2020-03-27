using System;
using System.IO;
using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;

namespace dotenv.net
{
    public class DotEnv
    {
        private const int LevelsToSearch = 4;
        private static DotEnv _instance;

        private static DotEnv Instance => _instance ?? (_instance = new DotEnv());

        private void ConfigRunner(bool throwOnError, string filePath, Encoding encoding, bool trimValues)
        {
            var rawEnvRows = Reader.Read(filePath, throwOnError, encoding);

            if (rawEnvRows == ReadOnlySpan<string>.Empty)
            {
                return;
            }

            var processedEnvRows = Parser.Parse(rawEnvRows, trimValues);
            foreach (var processedEnvRow in processedEnvRows)
            {
                Environment.SetEnvironmentVariable(processedEnvRow.Key, processedEnvRow.Value);
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

        public static bool AutoConfig()
        {
            
        }
    }
}