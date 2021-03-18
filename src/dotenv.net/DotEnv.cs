using System;
using System.Collections.Generic;
using dotenv.net.Utilities;

namespace dotenv.net
{
    public static class DotEnv
    {
        /// <summary>
        /// [Deprecated] Configure the environment variables from a .env file
        /// </summary>
        /// <param name="options">Options on how to load the env file</param>
        [Obsolete(
            "This method would be removed in the next major release. Use the Fluent API, Load() or Read() methods instead.")]
        public static void Config(DotEnvOptions options)
        {
            Helpers.ReadAndWrite(options);
        }

        /// <summary>
        /// [Deprecated] Searches the current directory and three directories up and loads the environment variables
        /// </summary>
        /// <param name="levelsToSearch">The number of top-level directories to search; the default is 4 top-level directories.</param>
        /// <returns>States whether or not the operation succeeded</returns>
        [Obsolete(
            "This method would be removed in the next major release. Use the Fluent API, Load() or Read() methods instead.")]
        public static bool AutoConfig(int levelsToSearch = DotEnvOptions.DefaultProbeDepth)
        {
            Helpers.ReadAndWrite(new DotEnvOptions(probeDirectoryDepth: levelsToSearch));
            return true;
        }

        /// <summary>
        /// Initialize the fluent configuration API
        /// </summary>
        public static DotEnvOptions Fluent()
        {
            return new();
        }

        /// <summary>
        /// Read and return the values in the provided env files
        /// </summary>
        /// <param name="options">The options required to configure the env loader</param>
        /// <returns>The key value pairs read from the env files</returns>
        public static IDictionary<string, string> Read(DotEnvOptions options)
        {
            return Helpers.ReadAndReturn(options);
        }

        /// <summary>
        /// Load the values in the provided env files into the environment variables
        /// </summary>
        /// <param name="options">The options required to configure the env loader</param>
        public static void Load(DotEnvOptions options)
        {
            Helpers.ReadAndWrite(options);
        }
    }
}