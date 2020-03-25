using System;
using dotenv.net.Interfaces;

namespace dotenv.net.Utilities
{
    public class EnvReader : IEnvReader
    {
        /// <summary>
        /// Retrieve a string value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>A string representing the value</returns>
        /// <exception cref="Exception">When the value could not be found</exception>
        public string GetStringValue(string key)
        {
            if (TryGetStringValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
        }

        /// <summary>
        /// Retrieve an integer value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>An integer representing the value</returns>
        /// <exception cref="Exception">When the value could not be found or is not an integer</exception>
        public int GetIntValue(string key)
        {
            if (TryGetIntValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
        }

        /// <summary>
        /// Retrieve a double value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>A double representing the value</returns>
        /// <exception cref="Exception">When the value could not be found or is not a valid double</exception>
        public double GetDoubleValue(string key)
        {
            if (TryGetDoubleValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
        }

        /// <summary>
        /// Retrieve a decimal value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>A decimal representing the value</returns>
        /// <exception cref="Exception">When the value could not be found or is not a valid decimal</exception>
        public decimal GetDecimalValue(string key)
        {
            if (TryGetDecimalValue(key, out var value))
            {
                return value;
            }

            throw new Exception("Value could not be retrieved.");
        }

        /// <summary>
        /// Retrieve a boolean value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <returns>A boolran representing the value</returns>
        /// <exception cref="Exception">When the value could not be found or is not a valid bool</exception>
        public bool GetBooleanValue(string key)
        {
            if (TryGetBooleanValue(key, out var value))
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

        /// <summary>
        /// Try to retrieve an int value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <param name="value">The int value retrieved or null</param>
        /// <returns>A value representing the retrieval success status</returns>
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

        /// <summary>
        /// Try to retrieve a double value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <param name="value">The double value retrieved or null</param>
        /// <returns>A value representing the retrieval success status</returns>
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

        /// <summary>
        /// Try to retrieve a decimal value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <param name="value">The decimal value retrieved or null</param>
        /// <returns>A value representing the retrieval success status</returns>
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

        /// <summary>
        /// Try to retrieve a boolean value from the current environment
        /// </summary>
        /// <param name="key">The key to retrieve the value via</param>
        /// <param name="value">The boolean value retrieved or null</param>
        /// <returns>A value representing the retrieval success status</returns>
        public bool TryGetBooleanValue(string key, out bool value)
        {
            var retrievedValue = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                return bool.TryParse(retrievedValue, out value);
            }

            value = false;
            return false;
        }
    }
}