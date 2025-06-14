using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace dotenv.net;

internal static class Parser
{
    private const char SingleQuote = '\'';
    private const char DoubleQuotes = '"';
    private const char BackSlash = '\\';

    internal static ReadOnlySpan<KeyValuePair<string, string>> Parse(ReadOnlySpan<string> rawEnvRows,
        bool trimValues)
    {
        var keyValuePairs = new List<KeyValuePair<string, string>>();

        for (var i = 0; i < rawEnvRows.Length; i++)
        {
            var rawEnvRow = rawEnvRows[i];

            if (string.IsNullOrWhiteSpace(rawEnvRow))
                continue;

            if (rawEnvRow.IsComment())
                continue;

            if (!rawEnvRow.HasKey(out var equalsIndex))
                continue;

            var (key, rawValue) = rawEnvRow.SplitIntoKv(equalsIndex);

            if (string.IsNullOrEmpty(key))
                continue;

            string value = string.Empty;
            var trimmedRawValue = rawValue.TrimStart();
            var isSingleQuoted = trimmedRawValue.StartsWith(SingleQuote);
            var isDoubleQuoted = trimmedRawValue.StartsWith(DoubleQuotes);

            if (isSingleQuoted || isDoubleQuoted)
            {
                var valueBuilder = new StringBuilder();
                var quoteEnded = false;
                var valueToUse = trimmedRawValue;

                while (!quoteEnded)
                {
                    for (var j = 0; j < valueToUse.Length; j++)
                    {
                        var currentChar = valueToUse[j];

                        if (isSingleQuoted && currentChar == SingleQuote ||
                            isDoubleQuoted && currentChar == DoubleQuotes)
                        {
                            // we check to see if the previous character was a backslash and the character before that was not a backslash
                            var previousChar = valueBuilder.GetCharAtIndexFromEnd(0);
                            var twoCharsBack = valueBuilder.GetCharAtIndexFromEnd(1);

                            if (previousChar is BackSlash && twoCharsBack is not BackSlash)
                            {
                                // remove the most recent backslash and append the current character
                                valueBuilder.RemoveLast();
                                valueBuilder.Append(currentChar);
                            }
                            else if (j == 0)
                            {
                                // do nothing, this is the first character and we are still inside the quotes
                            }
                            else if (j != valueToUse.Length - 1)
                            {
                                throw new InvalidOperationException(
                                    $"Unable to parse environment variable: {key}. Unexpected closing quote before row end.");
                            }
                            else
                            {
                                quoteEnded = true;
                                value = valueBuilder.ToString();
                                break;
                            }
                        }
                        else
                        {
                            valueBuilder.Append(currentChar);
                        }
                    }

                    if (!quoteEnded)
                    {
                        i += 1;

                        if (i >= rawEnvRows.Length)
                            throw new ArgumentException(
                                $"Unable to parse environment variable: {key}. Missing closing quote.");

                        valueToUse = rawEnvRows[i];
                        valueBuilder.AppendLine();
                    }
                }
            }
            else
            {
                value = rawValue;
            }

            if (trimValues)
                value = value.Trim();

            keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
        }

        return keyValuePairs.ToArray();
    }

    private static bool IsComment(this string value) => value.StartsWith("#");

    private static bool HasKey(this string value, out int index)
    {
        index = value.IndexOf('=');
        return index > 0;
    }

    private static (string Key, string Value) SplitIntoKv(this string rawEnvRow, int index)
    {
        var key = rawEnvRow.Substring(0, index).Trim();
        var value = rawEnvRow.Substring(index + 1);
        return (key, value);
    }

    private static bool StartsWith(this string input, char character) =>
        !string.IsNullOrEmpty(input) && input[0] == character;

    private static void RemoveLast(this StringBuilder sb) => sb.Remove(sb.Length - 1, 1);

    private static char? GetCharAtIndexFromEnd(this StringBuilder sb, int indexFromEnd)
    {
        if (sb.Length < indexFromEnd + 1)
            return null;

        return sb[sb.Length - indexFromEnd - 1];
    }
}
