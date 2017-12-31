using System.Text;

namespace dotenv.net.DependencyInjection.Infrastructure
{
    public class DotEnvOptionsBuilder
    {
        private readonly DotEnvOptions _dotEnvOptions = new DotEnvOptions();
        
        public DotEnvOptionsBuilder AddEnvFile(string file)
        {
            _dotEnvOptions.EnvFile = file;
            return this;
        }

        public DotEnvOptionsBuilder AddThrowOnError(bool throwOnError)
        {
            _dotEnvOptions.ThrowOnError = throwOnError;
            return this;
        }

        public DotEnvOptionsBuilder AddEncoding(Encoding encoding)
        {
            _dotEnvOptions.Encoding = encoding;
            return this;
        }
    }
}