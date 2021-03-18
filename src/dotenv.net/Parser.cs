using System;
using System.Collections.Generic;

namespace dotenv.net
{
    internal static class Parser
    {
        private static readonly char[] SingleQuote = {'\''};
        private static readonly char[] DoubleQuotes = {'"'};
        
        internal static ReadOnlySpan<KeyValuePair<string, string>> Parse(ReadOnlySpan<string> dotEnvRows,
            bool shouldTrimValue)
        {
            var validEntries = new List<KeyValuePair<string, string>>();

            foreach (var dotEnvRow in dotEnvRows)
            {
                var row = new ReadOnlySpan<char>(dotEnvRow.TrimStart().ToCharArray());

                if (row.IsEmpty)
                    continue;

                if (row.IsComment())
                    continue;

                if (row.HasNoKey(out var index))
                    continue;

                var key = row.Key(index);
                var value = row.Value(index, shouldTrimValue);
                validEntries.Add(new KeyValuePair<string, string>(key, value));
            }

            return new ReadOnlySpan<KeyValuePair<string, string>>(validEntries.ToArray());
        }

        private static bool IsComment(this ReadOnlySpan<char> row) => row[0] == '#';

        private static bool HasNoKey(this ReadOnlySpan<char> row, out int index)
        {
            index = row.IndexOf('=');
            return index <= 0;
        }

        private static bool IsQuoted(this ReadOnlySpan<char> row) => (row.StartsWith(SingleQuote) && row.EndsWith(SingleQuote))
                                                                     || (row.StartsWith(DoubleQuotes) && row.EndsWith(DoubleQuotes));

        private static ReadOnlySpan<char> StripQuotes(this ReadOnlySpan<char> row) => row.Trim('\'').Trim('\"');

        private static string Key(this ReadOnlySpan<char> row, int index)
        {
            var untrimmedKey = row.Slice(0, index);
            return untrimmedKey.Trim().ToString();
        }

        private static string Value(this ReadOnlySpan<char> row, int index, bool trimValue)
        {
            var value = row.Slice(index + 1);

            // handle quoted values
            if (value.IsQuoted())
            {
                value = value.StripQuotes();
            }

            // trim output if requested
            if (trimValue)
            {
                value = value.Trim();
            }

            return value.ToString();
        }
    }
}