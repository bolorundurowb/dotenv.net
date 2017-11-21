using System;
using System.IO;
using System.Text;

namespace dotenv.net
{
    public class DotEnv
    {
        public static void Config(bool throwOnError = true, string filePath = ".env", Encoding encoding = null)
        {
            if (throwOnError && !File.Exists(filePath))
            {
                throw new FileNotFoundException("Environment file specified does not exist.");
            }

            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            string dotEnvContents = File.ReadAllText(filePath, encoding);
        }
    }
}