using System;
using System.IO;
using System.Text;

namespace dotenv.net
{
    internal static class Reader
    {
        internal static ReadOnlySpan<string> Read(string envFilePath, bool ignoreExceptions, Encoding encoding)
        {
            var defaultResponse = ReadOnlySpan<string>.Empty;

            // if configured to throw errors then throw otherwise return
            if (string.IsNullOrWhiteSpace(envFilePath))
            {
                if (ignoreExceptions)
                {
                    return defaultResponse;
                }

                throw new ArgumentException("The file path cannot be null, empty or whitespace.", nameof(envFilePath));
            }

            // if configured to throw errors then throw otherwise return
            if (!File.Exists(envFilePath))
            {
                if (ignoreExceptions)
                {
                    return defaultResponse;
                }

                throw new FileNotFoundException($"A file with provided path \"{envFilePath}\" does not exist.");
            }

            // default to UTF8 if encoding is not provided
            encoding ??= Encoding.UTF8;

            // read all lines from the env file
            return new ReadOnlySpan<string>(File.ReadAllLines(envFilePath, encoding));
        }
    }
}