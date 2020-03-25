using System;
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
            if (TryGetStringValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
        }

        public int GetIntValue(string key)
        {
            if (TryGetIntValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
        }

        public double GetDoubleValue(string key)
        {
            if (TryGetDoubleValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
        }

        public decimal GetDecimalValue(string key)
        {
            if (TryGetDecimalValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
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

        public bool TryGetIntValue(string key, out int value)
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                return int.TryParse(retrievedValue, out value);
            }

            value = 0;
            return false;
        }

        public bool TryGetDoubleValue(string key, out double value)
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                return double.TryParse(retrievedValue, out value);
            }

            value = 0.0;
            return false;
        }

        public bool TryGetDecimalValue(string key, out decimal value)
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                return decimal.TryParse(retrievedValue, out value);
            }

            value = 0.0m;
            return false;
        }
    }
}