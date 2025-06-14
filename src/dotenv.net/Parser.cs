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

    private static readonly Regex IsQuotedLineStart = new("^[a-zA-Z0-9_ .-]+=\\s*\".*$", RegexOptions.Compiled);
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
                                 // ignore
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
                            throw new ArgumentException($"Unable to parse environment variable: {key}. Missing closing quote.");
                        
                        valueToUse = rawEnvRows[i];
                        valueBuilder.AppendLine();
                    }
                }
            }
            else
            {
                value = rawValue;
            }
            
            

            // string value;
            // if (IsQuotedLineStart.IsMatch(rawEnvRow))
            // {
            //     var valueBuilder = new StringBuilder(rawValue);
            //
            //     while (!IsQuotedLineEnd.IsMatch(rawEnvRow))
            //     {
            //         i += 1;
            //
            //         if (i >= rawEnvRows.Length)
            //             break;
            //
            //         rawEnvRow = rawEnvRows[i];
            //         valueBuilder.AppendLine();
            //         valueBuilder.Append(rawEnvRow);
            //     }
            //
            //     value = valueBuilder.ToString();
            // }
            // else
            // {
            //     value = rawValue;
            // }

            // value = StripQuotes(value);

            if (trimValues)
                value = value.Trim();

            keyValuePairs.Add(new KeyValuePair<string, string>(key, value));
        }

        return keyValuePairs.ToArray();
    }

    private static bool IsComment(this string value) => value.StartsWith("#");

    // private static string StripQuotes(this string value)
    // {
    //     var trimmed = value.Trim();
    //     var modified = false;
    //
    //     if (trimmed.Length > 1 &&
    //         ((trimmed.StartsWith(DoubleQuotes) && trimmed.EndsWith(DoubleQuotes)) ||
    //          (trimmed.StartsWith(SingleQuote) && trimmed.EndsWith(SingleQuote))))
    //     {
    //         trimmed = trimmed.Substring(1, trimmed.Length - 2);
    //         modified = true;
    //     }
    //
    //
    //     return modified ? trimmed : value;
    // }

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
    
    private static bool StartsWith(this string input, char character) => !string.IsNullOrEmpty(input) && input[0] == character;
    
    private static void RemoveLast(this StringBuilder sb) => sb.Remove(sb.Length - 1, 1);

    private static char? GetCharAtIndexFromEnd(this StringBuilder sb, int indexFromEnd)
    {
        if (sb.Length < indexFromEnd + 1)
            return null;

        return sb[sb.Length - indexFromEnd - 1];
    }
}
