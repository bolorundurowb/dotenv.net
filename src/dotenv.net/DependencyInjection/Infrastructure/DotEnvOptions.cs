using System.Collections.Generic;
using System.Text;

namespace dotenv.net.DependencyInjection.Infrastructure
{
    public class DotEnvOptions
    {
        /// <summary>
        /// A value to state whether to throw an exception if the env file doesn't exist. The default is true. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/>
        /// </summary>
        public bool IgnoreExceptions { get; set; }

        /// <summary>
        /// The paths to the env files. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/>
        /// </summary>
        public IEnumerable<string> EnvFilePaths { get; set; }

        /// <summary>
        /// The Encoding that the env file was created with. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/>
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// A value to state whether or not to trim whitespace from the values retrieved. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/>
        /// </summary>
        public bool TrimValues { get; set; }
    }
}
