using System.Collections.Generic;
using System.Text;

namespace dotenv.net.DependencyInjection.Infrastructure
{
    public class DotEnvOptionsBuilder
    {
        private readonly DotEnvOptions _dotEnvOptions = new();

        /// <summary>
        /// Sets the environment file to be read <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptionsBuilder"/>
        /// </summary>
        /// <param name="envFilePath">The env file path</param>
        /// <returns>The current options builder</returns>
        public DotEnvOptionsBuilder AddEnvFile(string envFilePath)
        {
            _dotEnvOptions.EnvFilePaths = new[] {envFilePath};
            return this;
        }

        /// <summary>
        /// Sets the environment files to be read <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptionsBuilder"/>
        /// </summary>
        /// <param name="envFilePaths">The env file paths</param>
        /// <returns>The current options builder</returns>
        public DotEnvOptionsBuilder AddEnvFiles(IEnumerable<string> envFilePaths)
        {
            _dotEnvOptions.EnvFilePaths = envFilePaths;
            return this;
        }

        /// <summary>
        /// Sets the option to throw an exception if an error should occur <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptionsBuilder"/>
        /// </summary>
        /// <param name="ignoreExceptions">A boolean determining if exceptions should be thrown</param>
        /// <returns>The current options builder</returns>
        public DotEnvOptionsBuilder AddIgnoreExceptionOptions(bool ignoreExceptions)
        {
            _dotEnvOptions.IgnoreExceptions = ignoreExceptions;
            return this;
        }

        /// <summary>
        /// Set the encoding to read the env file in. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptionsBuilder"/>
        /// </summary>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>The current options builder</returns>
        public DotEnvOptionsBuilder AddEncoding(Encoding encoding)
        {
            _dotEnvOptions.Encoding = encoding;
            return this;
        }

        /// <summary>
        /// Set the option to trim whitespace from values. <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptionsBuilder"/>
        /// </summary>
        /// <param name="trimValues">A boolean determining whether values read should be trimmed</param>
        /// <returns>The current options builder</returns>
        public DotEnvOptionsBuilder AddTrimOptions(bool trimValues)
        {
            _dotEnvOptions.TrimValues = trimValues;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The constructed <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptions"/></returns>
        public DotEnvOptions Build()
        {
            return _dotEnvOptions;
        }
    }
}
