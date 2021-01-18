using System;
using System.Collections.Generic;

namespace dotenv.net
{
    internal static class Parser
    {
        internal static ReadOnlySpan<KeyValuePair<string, string>> Parse(ReadOnlySpan<string> dotEnvRows,
            bool shouldTrimValue)
        {
            var validEntries = new List<KeyValuePair<string, string>>();

            // loop through rows, split into key and value then add to environment
            foreach (var dotEnvRow in dotEnvRows)
            {
                var rowSpan = new ReadOnlySpan<char>(dotEnvRow.TrimStart().ToCharArray());

                if (rowSpan.IsEmpty)
                    continue;

                if (rowSpan.IsComment())
                    continue;

                if (rowSpan.HasNoKey(out var index))
                    continue;

                var untrimmedKey = rowSpan.Slice(0, index);
                var untrimmedValue = rowSpan.Slice(index + 1);
                var key = untrimmedKey.Trim().ToString();
                var value = untrimmedValue.ToString();

                // handle quoted values
                if (value.StartsWith("'") && value.EndsWith("'"))
                {
                    value = value.Trim('\'');
                }
                else if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Trim('\"');
                }

                // trim output if requested
                if (shouldTrimValue)
                {
                    value = value.Trim();
                }

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

        private static string Key(this ReadOnlySpan<char> row, int index)
        {
            var untrimmedKey = row.Slice(0, index);
            return untrimmedKey.Trim().ToString();
        }

        private static string Value(this ReadOnlySpan<char> row, int index, bool trimValue = false)
        {
            var untrimmedValue = row.Slice(index + 1);
            var value = untrimmedValue.ToString();
            
            // handle quoted values
            if (value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Trim('\'');
            }
            else if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Trim('\"');
            }

            // trim output if requested
            if (trimValue)
            {
                value = value.Trim();
            }

            return value;
        }
    }
}