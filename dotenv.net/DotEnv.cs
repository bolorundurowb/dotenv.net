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

            // read all lines from the env file
            string dotEnvContents = File.ReadAllText(filePath, encoding);
            
            // split the long string into an array of rows
            string[] dotEnvRows = dotEnvContents.Split(new[] {"\r\n", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            
            // loop through rows, split into key and value then add to enviroment
            foreach (var dotEnvRow in dotEnvRows)
            {
                string[] keyValue = dotEnvRow.Split(new[] {"="}, StringSplitOptions.None);
                
                // if the row is empty continue
                if (keyValue.Length == 0) 
                {
                    continue;
                }
                
                // if there is only a key add a null value
                else if (keyValue.Length == 1)
                {
                    Environment.SetEnvironmentVariable(keyValue[0].Trim(), null);
                }

                // if the value exists then set it
                else
                {
                    Environment.SetEnvironmentVariable(keyValue[0].Trim(), keyValue[1].Trim());
                }
            }
        }
    }
}