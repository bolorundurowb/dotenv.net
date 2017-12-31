using System.Text;

namespace dotenv.net.DependencyInjection.Infrastructure
{
    public class DotEnvOptions
    {
        /// <summary>
        /// A value to state whether to throw an exception if the env file doesn't exist. The default is true. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/>
        /// </summary>
        public bool ThrowOnError { get; set; } = true;

        /// <summary>
        /// The path to the env file. The default is ".env". <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/>
        /// </summary>
        public string EnvFile { get; set; } = ".env";

        /// <summary>
        /// The Encoding that the env file was created with. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/>
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;
    }
}