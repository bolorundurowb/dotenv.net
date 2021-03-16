using System.Collections.Generic;
using System.Text;

namespace dotenv.net
{
    public class DotEnvOptions
    {
        private const string DefaultEnvFileName = ".env";
        private const int DefaultProbeDepth = 4;

        /// <summary>
        /// A value to state whether to throw or swallow exceptions. The default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool IgnoreExceptions { get; set; }

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
        public bool TrimValues { get; set; }

        /// <summary>
        /// A value to state whether or not to override the env variable if it has been set. the default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool AllowOverwriting { get; set; }

        /// <summary>
        /// A value to state whether we traverse up the directory structure. The default is false. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool ProbeForEnv { get; set; }

        /// <summary>
        /// Default constructor for the dot env options
        /// </summary>
        /// <param name="ignoreExceptions"></param>
        /// <param name="envFilePaths"></param>
        /// <param name="encoding"></param>
        /// <param name="trimValues"></param>
        /// <param name="allowOverwriting"></param>
        /// <param name="probeForEnv"></param>
        public DotEnvOptions(bool ignoreExceptions = true, IEnumerable<string> envFilePaths = null,
            Encoding encoding = null, bool trimValues = false, bool allowOverwriting = true, bool probeForEnv = false)
        {
            IgnoreExceptions = ignoreExceptions;
            EnvFilePaths = envFilePaths ?? new[] {DefaultEnvFileName};
            Encoding = encoding ?? Encoding.UTF8;
            TrimValues = trimValues;
            AllowOverwriting = allowOverwriting;
            ProbeForEnv = probeForEnv;
        }
    }
}