using System;
using System.ComponentModel;
using dotenv.net.Interfaces;

namespace dotenv.net.Utilities
{
    public class EnvReader : IEnvReader
    {
        /// <summary>
        /// Retrieve a value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>A string representing the value if it exists or null</returns>
        public string GetStringValue(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        /// <summary>
        /// Try to retrieve a value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <param name="value">The string value retrieved or null</param>
        /// <returns>A value representing the retrieval success status</returns>
        public bool TryGetStringValue(string key, out string value)
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                value = retrievedValue;
                return true;
            }

            value = null;
            return false;
        }
    }
}