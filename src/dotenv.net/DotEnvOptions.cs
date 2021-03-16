using System.Collections.Generic;
using System.Text;

namespace dotenv.net
{
    public class DotEnvOptions
    {
        private const string DefaultEnvFileName = ".env";

        /// <summary>
        /// A value to state whether to throw an exception if the env file doesn't exist. The default is true. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool IgnoreExceptions { get; set; }

        /// <summary>
        /// The paths to the env files. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public IEnumerable<string> EnvFilePaths { get; set; }

        /// <summary>
        /// The Encoding that the env file was created with. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// A value to state whether or not to trim whitespace from the values retrieved. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool TrimValues { get; set; }

        /// <summary>
        /// A value to state whether or not to override the env variable if it has been set. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool OverrideExistingVariables { get; set; }

        /// <summary>
        /// A value to state whether we traverse up the directory structure. <see cref="T:dotenv.net.DotEnvOptions"/>
        /// </summary>
        public bool ProbePath { get; set; }

        public DotEnvOptions()
        {
            IgnoreExceptions = true;
            EnvFilePaths = new[] {DefaultEnvFileName};
            Encoding = Encoding.UTF8;
            TrimValues = true;
            OverrideExistingVariables = true;
            ProbePath = false;
        }
    }
}