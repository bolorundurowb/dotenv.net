﻿using System.Collections.Generic;
using System.Text;

namespace dotenv.net
{
    public class DotEnvOptions
    {
        public const string DefaultEnvFileName = ".env";
        private const int DefaultProbeDepth = 4;

        /// <summary>
        /// A value to state whether to throw or swallow exceptions. The default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool ShouldIgnoreExceptions { get; set; }

        /// <summary>
        /// The paths to the env files. The default is [.env] <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public IEnumerable<string> EnvFilePaths { get; set; }

        /// <summary>
        /// The Encoding that the env file was created with. The default is UTF-8. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// A value to state whether or not to trim whitespace from the values retrieved. The default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool ShouldTrimValues { get; set; }

        /// <summary>
        /// A value to state whether or not to override the env variable if it has been set. the default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool ShouldOverwriteExistingVars { get; set; }

        /// <summary>
        /// A value to state whether we traverse up the directory structure. The default is false. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool ShouldProbeForEnv { get; set; }

        /// <summary>
        /// A value to state how far up the directory structure we should search for env files. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public int ProbeDirectoryDepth { get; set; }

        /// <summary>
        /// Default constructor for the dot env options
        /// </summary>
        /// <param name="ignoreExceptions">A boolean </param>
        /// <param name="envFilePaths"></param>
        /// <param name="encoding"></param>
        /// <param name="trimValues"></param>
        /// <param name="overwriteExistingVars"></param>
        /// <param name="probeForEnv"></param>
        /// <param name="probeDirectoryDepth"></param>
        public DotEnvOptions(bool ignoreExceptions = true, IEnumerable<string> envFilePaths = null,
            Encoding encoding = null, bool trimValues = false, bool overwriteExistingVars = true,
            bool probeForEnv = false, int probeDirectoryDepth = DefaultProbeDepth)
        {
            ShouldIgnoreExceptions = ignoreExceptions;
            EnvFilePaths = envFilePaths ?? new[] {DefaultEnvFileName};
            Encoding = encoding ?? Encoding.UTF8;
            ShouldTrimValues = trimValues;
            ShouldOverwriteExistingVars = overwriteExistingVars;
            ShouldProbeForEnv = probeForEnv;
            ProbeDirectoryDepth = probeDirectoryDepth;
        }

        /// <summary>
        /// Ignore exceptions thrown
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions IgnoreExceptions()
        {
            ShouldIgnoreExceptions = true;
            return this;
        }

        /// <summary>
        /// Throw exceptions when triggered
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions ThrowExceptions()
        {
            ShouldIgnoreExceptions = false;
            return this;
        }

        /// <summary>
        /// Search up the directory for a .env file. By default searches up 4 directories.
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions ProbeForEnv(int probeDepth = DefaultProbeDepth)
        {
            ShouldProbeForEnv = true;
            ProbeDirectoryDepth = probeDepth;
            return this;
        }

        /// <summary>
        /// Rely on the provided env files. By default is false.
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions NoProbeForEnv()
        {
            ShouldProbeForEnv = true;
            ProbeDirectoryDepth = DefaultProbeDepth;
            return this;
        }

        /// <summary>
        /// Overwrite an environment variable even if it has been set
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions OverwriteExistingVars()
        {
            ShouldOverwriteExistingVars = true;
            return this;
        }

        /// <summary>
        /// Only write an environment variable if it hasnt been et
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions NoOverwriteExistingVars()
        {
            ShouldOverwriteExistingVars = false;
            return this;
        }

        /// <summary>
        /// Trim whitespace from the values read
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions TrimValues()
        {
            ShouldTrimValues = true;
            return this;
        }

        /// <summary>
        /// Leave read values as is
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions NoTrimValues()
        {
            ShouldTrimValues = false;
            return this;
        }

        /// <summary>
        /// Change the encoding for reading the env files
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions SetEncoding(Encoding encoding)
        {
            Encoding = encoding ?? Encoding.UTF8;
            return this;
        }

        /// <summary>
        /// Revert to the default encoding for reading the env files. The default encoding is UTF-8
        /// </summary>
        /// <returns>configured dot env options</returns>
        public DotEnvOptions DefaultEncoding()
        {
            Encoding = Encoding.UTF8;
            return this;
        }

        /// <summary>
        /// Return the values in the env files without writing to the environment
        /// </summary>
        /// <returns>configured dot env options</returns>
        public IDictionary<string, string> Read() => DotEnv.Read(this);

        /// <summary>
        /// Read the env files and write to the syetm environment variables
        /// </summary>
        public void Load() => DotEnv.Load(this);
    }
}