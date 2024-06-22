using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace dotenv.net;

internal static class Parser
{
    private const string SingleQuote = "'";
    private const string DoubleQuotes = "\"";

    private static readonly Regex IsQuotedLineStart = new("^[a-zA-Z0-9_ ]+=\\s*\".*$", RegexOptions.Compiled);
    private static readonly Regex IsQuotedLineEnd = new("(?<!\\\\)\"\\s*$", RegexOptions.Compiled);

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
            string value;

            if (string.IsNullOrEmpty(key))
                continue;

            if (IsQuotedLineStart.IsMatch(rawEnvRow))
            {
                var valueBuilder = new StringBuilder(rawValue);

                while (!IsQuotedLineEnd.IsMatch(rawEnvRow))
                {
                    i += 1;

                    if (i >= rawEnvRows.Length)
                        break;

                    rawEnvRow = rawEnvRows[i];
                    valueBuilder.AppendLine();
                    valueBuilder.Append(rawEnvRow);
                }

                value = valueBuilder.ToString();
            }
            else
            {
                value = rawValue;
            }

            value = StripQuotes(value);

            if (trimValues)
                value = value.Trim();

            keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
        }

        return keyValuePairs.ToArray();
    }

    private static bool IsComment(this string value) => value.StartsWith("#");

    private static string StripQuotes(this string value)
    {
        var trimmed = value.Trim();
        var modified = false;

        if (trimmed.Length > 1 &&
            ((trimmed.StartsWith(DoubleQuotes) && trimmed.EndsWith(DoubleQuotes)) ||
             (trimmed.StartsWith(SingleQuote) && trimmed.EndsWith(SingleQuote))))
        {
            trimmed = trimmed.Substring(1, trimmed.Length - 2);
            modified = true;
        }


        return modified ? trimmed : value;
    }

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
}
