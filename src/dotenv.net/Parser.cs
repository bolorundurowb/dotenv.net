using System;
using System.Collections.Generic;
using System.Text;

namespace dotenv.net
{
    internal static class Parser
    {
        private const string SingleQuote = "'";
        private const string DoubleQuotes = "\"";

        internal static ReadOnlySpan<KeyValuePair<string, string>> Parse(ReadOnlySpan<string> rawEnvRows,
            bool trimValues)
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>();

            for (var i = 0; i < rawEnvRows.Length; i++)
            {
                var rawEnvRow = rawEnvRows[i];

                if(rawEnvRow.StartsWith("#")) continue;

                if (rawEnvRow.Contains("=\""))
                {
                    var key = rawEnvRow.Substring(0, rawEnvRow.IndexOf("=\"", StringComparison.Ordinal));
                    var valueStringBuilder = new StringBuilder();
                    valueStringBuilder.Append(rawEnvRow.Substring(rawEnvRow.IndexOf("=\"", StringComparison.Ordinal) + 2));

                    while (!rawEnvRow.EndsWith("\""))
                    {
                        i++;
                        if (i >= rawEnvRows.Length)
                        {
                            break;
                        }
                        rawEnvRow = rawEnvRows[i];
                        valueStringBuilder.Append(rawEnvRow);
                    }
                    //Remove last "
                    valueStringBuilder.Remove(valueStringBuilder.Length - 1, 1);

                    var value = valueStringBuilder.ToString();
                    if (trimValues)
                    {
                        value = value.Trim();
                    }

                    keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
                }
                else
                {
                    //Check that line is not empty
                    var rawEnvEmpty = rawEnvRow.Trim();
                    if(string.IsNullOrEmpty(rawEnvEmpty)) continue;

                    // Regular key-value pair
                    var keyValue = rawEnvRow.Split(new[] { '=' }, 2);

                    var key = keyValue[0].Trim();
                    var value = keyValue[1];

                    if(string.IsNullOrEmpty(key)) continue;

                    if (IsQuoted(value))
                    {
                        value = StripQuotes(value);
                    }

                    if (trimValues)
                    {
                        value = value.Trim();
                    }

                    keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            return keyValuePairs.ToArray();
        }

        private static bool IsQuoted(string value) => (value.StartsWith(SingleQuote) && value.EndsWith(SingleQuote))
                                                                     || (value.StartsWith(DoubleQuotes) && value.EndsWith(DoubleQuotes));

        private static string StripQuotes(string value)
        {
            return value.Substring(1, value.Length - 2);
        }
    }
}