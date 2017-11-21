using System;
using System.IO;
using System.Text;

namespace dotenv.net
{
    public class DotEnv
    {
        private static readonly string DefaultEnvFilePath = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}{Path.DirectorySeparatorChar}.env";

        public static void Config(bool throwOnError = true, string filePath = null, Encoding encoding = null)
        {
            if (filePath == null)
            {
                filePath = DefaultEnvFilePath;
            }
            
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
            string[] dotEnvRows = dotEnvContents.Split(new[] {"\r\n", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            
            // loop through rows, split into key and value then add to enviroment
            foreach (var dotEnvRow in dotEnvRows)
            {
                string[] keyValue = dotEnvRow.Split(new[] {"="}, StringSplitOptions.None);
                
                // if the row is empty continue
                switch (keyValue.Length)
                {
                    case 0:
                        break;
                    case 1:
                        Environment.SetEnvironmentVariable(keyValue[0].Trim(), null);
                        break;
                    default:
                        Environment.SetEnvironmentVariable(keyValue[0].Trim(), keyValue[1].Trim());
                        break;
                }
            }
        }
    }
}