using System.Text;

namespace dotenv.net.DependencyInjection.Infrastructure
{
    public class DotEnvOptionsBuilder
    {
        private readonly DotEnvOptions _dotEnvOptions = new DotEnvOptions();

        /// <summary>
        /// Sets the environment file to be read <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptionsBuilder"/>
        /// </summary>
        /// <param name="file">The file path</param>
        /// <returns>The current options builder</returns>
        public DotEnvOptionsBuilder AddEnvFile(string file)
        {
            _dotEnvOptions.EnvFile = file;
            return this;
        }

        /// <summary>
        /// Sets the option to throw an eception if an error should occur <see cref="T:dotenv.net.DependencyInjection.Infrastructure.DotEnvOptionsBuilder"/>
        /// </summary>
        /// <param name="throwOnError">A boolean determining if eeceptions should be thrown</param>
        /// <returns>The current options builder</returns>
        public DotEnvOptionsBuilder AddThrowOnError(bool throwOnError)
        {
            _dotEnvOptions.ThrowOnError = throwOnError;
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
        /// <param name="trimValues">The encoding to use</param>
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
