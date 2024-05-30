using System;

namespace dotenv.net.Utilities;

/// <summary>
/// Holds reader helper methods
/// </summary>
public static class EnvReader
{
    /// <summary>
    /// Retrieve a string value from the current environment
    /// </summary>
    /// <param name="key">The key to retrieve the value via</param>
    /// <returns>A string representing the value</returns>
    /// <exception cref="Exception">When the value could not be found</exception>
    public static string GetStringValue(string key)
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
    public static int GetIntValue(string key)
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
    public static double GetDoubleValue(string key)
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
    public static decimal GetDecimalValue(string key)
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
    /// <returns>A boolean representing the value</returns>
    /// <exception cref="Exception">When the value could not be found or is not a valid bool</exception>
    public static bool GetBooleanValue(string key)
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
    public static bool TryGetStringValue(string key, out string value)
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
    public static bool TryGetIntValue(string key, out int value)
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
    public static bool TryGetDoubleValue(string key, out double value)
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
    public static bool TryGetDecimalValue(string key, out decimal value)
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
    public static bool TryGetBooleanValue(string key, out bool value)
    {
            var retrievedValue = Environment.GetEnvironmentVariable(key);

            if (!string.IsNullOrEmpty(retrievedValue))
            {
                return bool.TryParse(retrievedValue, out value);
            }

            value = false;
            return false;
        }

    /// <summary>
    /// Determine if an environment key has a set value or not
    /// </summary>
    /// <param name="key">The key to retrieve the value via</param>
    /// <returns>A value determining if a value is set or not</returns>
    public static bool HasValue(string key)
    {
            var retrievedValue = Environment.GetEnvironmentVariable(key);
            return !string.IsNullOrEmpty(retrievedValue);
        }
}