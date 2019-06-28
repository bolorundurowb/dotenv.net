using System;
using System.IO;
using System.Text;

namespace dotenv.net
{
    internal static class Reader
    {
        internal static ReadOnlySpan<string> Read(string filePath, bool throwOnError, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("The file path cannot be null, empty or whitespace", nameof(filePath));
            }

            // if configured to throw errors then throw otherwise return
            if (!File.Exists(filePath))
            {
                if (throwOnError)
                {
                    throw new FileNotFoundException($"An environment file with path \"{filePath}\" does not exist.");
                }

                return ReadOnlySpan<string>.Empty;
            }

            // default to UTF8 if null
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            // read all lines from the env file
            return new ReadOnlySpan<string>(File.ReadAllLines(filePath, encoding));
        }
    }
}