using System;
using System.IO;
using System.Text;

namespace dotenv.net
{
    internal static class Reader
    {
        internal static ReadOnlySpan<string> Read(string envFilePath, bool ignoreExceptions, Encoding encoding)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(envFilePath))
                {
                    throw new ArgumentException("The file path cannot be null, empty or whitespace.", nameof(envFilePath));
                }

                // if configured to throw errors then throw otherwise return
                if (!File.Exists(envFilePath))
                {
                    throw new FileNotFoundException(
                        $"An environment file with path \"{envFilePath}\" does not exist.");
                }

                // default to UTF8 if encoding is not provided
                encoding ??= Encoding.UTF8;

                // read all lines from the env file
                return new ReadOnlySpan<string>(File.ReadAllLines(envFilePath, encoding));
            }
            catch (Exception)
            {
                if (ignoreExceptions)
                {
                    return ReadOnlySpan<string>.Empty;
                }

                throw;
            }
        }
    }
}