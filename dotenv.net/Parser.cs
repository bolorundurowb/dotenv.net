using System;
using System.Collections.Generic;

namespace dotenv.net
{
    internal static class Parser
    {
        internal static ReadOnlySpan<KeyValuePair<string, string>> Parse(ReadOnlySpan<string> dotEnvRows,
            bool trimValues)
        {
            var validEntries = new List<KeyValuePair<string, string>>();

            // loop through rows, split into key and value then add to environment
            foreach (var dotEnvRow in dotEnvRows)
            {
                var rowSpan = new ReadOnlySpan<char>(dotEnvRow.ToCharArray());

                // determine if row is empty
                if (rowSpan.Length == 0)
                    continue;

                // determine if row is comment
                if (rowSpan[0] == '#')
                    continue;

                var index = rowSpan.IndexOf('=');

                // if there is no key, skip
                if (index < 0)
                    continue;

                var untrimmedKey = rowSpan.Slice(0, index);
                var untrimmedValue = rowSpan.Slice(index + 1);
                var key = untrimmedKey.Trim();
                var value = trimValues ? untrimmedValue.Trim() : untrimmedValue;

                validEntries.Add(new KeyValuePair<string, string>(key.ToString(), value.ToString()));
            }

            return new ReadOnlySpan<KeyValuePair<string, string>>(validEntries.ToArray());
        }
    }
}