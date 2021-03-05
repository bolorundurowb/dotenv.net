using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;

namespace dotenv.net
{
    public class DotEnv
    {
        private const string DefaultEnvFileName = ".env";

        private static void ConfigRunner(bool throwOnError, string filePath, Encoding encoding, bool trimValues)
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
        public static void Config(bool throwOnError = true, string filePath = DefaultEnvFileName,
            Encoding encoding = null, bool trimValues = true)
        {
            ConfigRunner(throwOnError, filePath, encoding, trimValues);
        }

        /// <summary>
        /// Configure the environment variables from a .env file
        /// </summary>
        /// <param name="options">Options on how to load the env file</param>
        public static void Config(DotEnvOptions options)
        {
            ConfigRunner(options.ThrowOnError, options.EnvFile, options.Encoding, options.TrimValues);
        }

        /// <summary>
        /// Searches the current directory and three directories up and loads the environment variables
        /// </summary>
        /// <param name="levelsToSearch">The number of top-level directories to search; the default is 4 top-level directories.</param>
        /// <returns>States whether or not the operation succeeded</returns>
        public static bool AutoConfig(int levelsToSearch = 4)
        {
            var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

            for (;
                currentDirectory != null && levelsToSearch > 0;
                levelsToSearch--, currentDirectory = currentDirectory.Parent)
            {
                foreach (var file in currentDirectory.GetFiles(DefaultEnvFileName, SearchOption.TopDirectoryOnly))
                {
                    Config(false, file.FullName);
                    return true;
                }
            }

            return false;
        }

        public static void Load(string envFilePath = DefaultEnvFileName, Encoding encoding = null,
            bool ignoreExceptions = true)
        {
            ConfigRunner(ignoreExceptions, envFilePath, encoding, true);
        }

        public static void Load(IEnumerable<string> envFilePaths = null, Encoding encoding = null,
            bool ignoreExceptions = true)
        {
            envFilePaths ??= Enumerable.Empty<string>();

            foreach (var envFilePath in envFilePaths)
            {
                ConfigRunner(ignoreExceptions, envFilePath, encoding, true);
            }
        }
    }
}