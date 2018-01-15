using System;
using System.IO;
using System.Text;
using dotenv.net.DependencyInjection.Infrastructure;

namespace dotenv.net
{
    public static class DotEnv
    {
        /// <summary>
        /// Configure the environment varibales from a .env file
        /// </summary>
        /// <param name="throwOnError">A value stating whether the application should throw an exception on unexpected data</param>
        /// <param name="filePath">An optional env file path, if not provided it defaults to the one in the same folder as the output exe or dll</param>
        /// <param name="encoding">The encoding with which the env file was created, It defaults to the platforms default</param>
        /// <exception cref="FileNotFoundException">Thrown if the env file doesn't exist</exception>
        public static void Config(bool throwOnError = true, string filePath = ".env", Encoding encoding = null)
        {
            // if configured to throw errors then throw otherwise return
            if (!File.Exists(filePath))
            {
                if (throwOnError)
                {
                    throw new FileNotFoundException("Environment file specified does not exist.");
                }
                return;
            }

            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            // read all lines from the env file
            string dotEnvContents = File.ReadAllText(filePath, encoding);
            
            // split the long string into an array of rows
            string[] dotEnvRows = dotEnvContents.Split(new[] {"\n", "\r\n", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            
            // loop through rows, split into key and value then add to enviroment
            foreach (var dotEnvRow in dotEnvRows)
            {
                int index = dotEnvRow.IndexOf( "=" );

                if ( index >= 0 )
                {
                    string key = dotEnvRow.Substring( 0, index).Trim();
                    string value = dotEnvRow.Substring( index + 1, dotEnvRow.Length - ( index + 1) ).Trim();
                
                    if ( key.Length > 0 ){
                        if ( value.Length == 0 ){
                            Environment.SetEnvironmentVariable( key, null );
                        } else {
                            Environment.SetEnvironmentVariable( key, value );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Configure the environment variables from a .env file
        /// </summary>
        /// <param name="options">Options on how to load the env file</param>
        public static void Config(DotEnvOptions options)
        {
            Config(options.ThrowOnError, options.EnvFile, options.Encoding);
        }
    }
}