using System;
using System.Collections.Generic;
using System.Text;

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

            var trimmedRawValue = rawValue.TrimStart();
            var isSingleQuoted = trimmedRawValue.StartsWith(SingleQuote);
            var isDoubleQuoted = trimmedRawValue.StartsWith(DoubleQuotes);

            var value = isSingleQuoted || isDoubleQuoted
                ? ParseQuotedValue(key, rawEnvRows, trimmedRawValue, ref i)
                : rawValue;

            if (trimValues)
                value = value.Trim();

            keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
        }

        return keyValuePairs.ToArray();
    }

    private static string ParseQuotedValue(string key, ReadOnlySpan<string> rawEnvRows, string currentRowValue,
        ref int i)
    {
        var quoteChar = currentRowValue[0];
        var valueBuilder = new StringBuilder();
        var currentLineContent = currentRowValue.Substring(1); // Start after the opening quote.

        while (true)
        {
            var endQuoteIndex = -1;
            var searchFrom = 0;

            // find the next unescaped quote on the current line.
            while (searchFrom < currentLineContent.Length)
            {
                var nextQuote = currentLineContent.IndexOf(quoteChar, searchFrom);

                // no more quotes on this line
                if (nextQuote == -1)
                    break;

                // count preceding backslashes to see if the quote is escaped.
                var backslashCount = 0;
                for (var j = nextQuote - 1; j >= 0 && currentLineContent[j] == BackSlash; j--)
                    backslashCount++;

                // an even number of backslashes means the quote is NOT escaped.
                if (backslashCount % 2 == 0)
                {
                    endQuoteIndex = nextQuote;
                    break;
                }

                // odd number of backslashes means it's escaped, continue searching
                searchFrom = nextQuote + 1;
            }

            if (endQuoteIndex != -1)
            {
                // closing quote found. Append the content before it and exit
                valueBuilder.Append(currentLineContent, 0, endQuoteIndex);
                break;
            }

            // no closing quote on this line, append the whole line and move to the next
            valueBuilder.Append(currentLineContent);
            i++;

            if (i >= rawEnvRows.Length)
                throw new ArgumentException(
                    $"Unable to parse environment variable: {key}. Missing closing quote.");

            valueBuilder.AppendLine();
            currentLineContent = rawEnvRows[i];
        }

        return valueBuilder.ToString()
            .UnescapeQuotes(quoteChar)
            .UnescapeBackslashes();
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

    private static string UnescapeQuotes(this string input, char quoteChar) =>
        input.Replace($"\\{quoteChar}", quoteChar.ToString());

    private static string UnescapeBackslashes(this string input) => input.Replace("\\\\", "\\");
}
