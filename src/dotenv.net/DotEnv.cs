using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dotenv.net
{
    public static class DotEnv
    {
        /// <summary>
        /// Initialize the fluent configuration API
        /// </summary>
        public static DotEnvOptions Config()
        {
            return new DotEnvOptions();
        }

        /// <summary>
        /// Configure the environment variables from a .env file
        /// </summary>
        /// <param name="options">Options on how to load the env file</param>
        [Obsolete]
        public static void Config(DotEnvOptions options)
        {
        }

        /// <summary>
        /// Searches the current directory and three directories up and loads the environment variables
        /// </summary>
        /// <param name="levelsToSearch">The number of top-level directories to search; the default is 4 top-level directories.</param>
        /// <returns>States whether or not the operation succeeded</returns>
        [Obsolete]
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
        public static Dictionary<string, string> Read(DotEnvOptions options)
        {
        }

        /// <summary>
        /// Load the values in the provided env file into the environment variables
        /// </summary>
        /// <param name="envFilePath">The path to the .env file to be read</param>
        /// <param name="encoding">The encoding that the env file was saved in</param>
        /// <param name="ignoreExceptions">Determines if an exception should be thrown or swallowed</param>
        public static void Load(DotEnvOptions options)
        {
            
        }
    }
}
