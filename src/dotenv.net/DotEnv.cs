using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        private static ReadOnlySpan<KeyValuePair<string, string>> ReadAndParse(string envFilePath,
            bool ignoreExceptions, Encoding encoding, bool trimValues)
        {
            var rawEnvRows = Reader.Read(envFilePath, ignoreExceptions, encoding);

            if (rawEnvRows == ReadOnlySpan<string>.Empty)
            {
                return ReadOnlySpan<KeyValuePair<string, string>>.Empty;
            }

            return Parser.Parse(rawEnvRows, trimValues);
        }

        private static IDictionary<string, string> ReadAndReturn(string envFilePath,
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

        private static void ReadAndWrite(string envFilePath,
            bool ignoreExceptions, Encoding encoding, bool trimValues)
        {
            var envRows = ReadAndParse(envFilePath, ignoreExceptions, encoding, trimValues);
            foreach (var envRow in envRows)
            {
                Environment.SetEnvironmentVariable(envRow.Key, envRow.Value);
            }
        }

        /// <summary>
        /// Initialize the fluent configuration API
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
            ConfigRunner(options.IgnoreExceptions, options.EnvFilePaths, options.Encoding, options.TrimValues);
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

        /// <summary>
        /// Read and return the values in the provided env file
        /// </summary>
        /// <param name="envFilePath">The path to the .env file to be read</param>
        /// <param name="encoding">The encoding that the env file was saved in</param>
        /// <param name="ignoreExceptions">Determines if an exception should be thrown or swallowed</param>
        public static Dictionary<string, string> Read(string envFilePath = DefaultEnvFileName, Encoding encoding = null,
            bool ignoreExceptions = true)
        {
            ConfigRunner(ignoreExceptions, envFilePath, encoding, true);
        }

        /// <summary>
        /// Read and return the values in the provided env files
        /// </summary>
        /// <param name="envFilePaths">The paths to the .env files to be read</param>
        /// <param name="encoding">The encoding that the env file was saved in</param>
        /// <param name="ignoreExceptions">Determines if an exception should be thrown or swallowed</param>
        /// <returns>An enumerable of dictionaries representing the contents of each env file</returns>
        public static IEnumerable<Dictionary<string, string>> Read(IEnumerable<string> envFilePaths = null, Encoding encoding = null,
            bool ignoreExceptions = true)
        {
            envFilePaths ??= Enumerable.Empty<string>();

            foreach (var envFilePath in envFilePaths)
            {
                ConfigRunner(ignoreExceptions, envFilePath, encoding, true);
            }
        }

        /// <summary>
        /// Load the values in the provided env file into the environment variables
        /// </summary>
        /// <param name="envFilePath">The path to the .env file to be read</param>
        /// <param name="encoding">The encoding that the env file was saved in</param>
        /// <param name="ignoreExceptions">Determines if an exception should be thrown or swallowed</param>
        public static void Load(string envFilePath = DefaultEnvFileName, Encoding encoding = null,
            bool ignoreExceptions = true)
        {
            ConfigRunner(ignoreExceptions, envFilePath, encoding, true);
        }

        /// <summary>
        /// Load the values in the provided env files into the environment variables
        /// </summary>
        /// <param name="envFilePaths">The paths to the .env files to be read</param>
        /// <param name="encoding">The encoding that the env file was saved in</param>
        /// <param name="ignoreExceptions">Determines if an exception should be thrown or swallowed</param>
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
