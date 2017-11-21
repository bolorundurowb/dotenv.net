using System;
using System.IO;

namespace dotenv.net
{
    public class DotEnv
    {
        public static void Config(bool throwOnError = true, string filePath = ".env")
        {
            if (throwOnError && !File.Exists(filePath))
            {
                throw new FileNotFoundException("Environment file specified does not exist.");
            }
            
        }
    }
}