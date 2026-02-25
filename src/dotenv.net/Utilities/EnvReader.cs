using System;

namespace dotenv.net.Utilities;

/// <summary>
/// Provides helper methods for reading strongly-typed values from the environment.
/// </summary>
public static class EnvReader
{
    /// <summary>
    /// Retrieves a string value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <returns>A string representing the value.</returns>
    /// <exception cref="Exception">Thrown when the value could not be found.</exception>
    public static string GetStringValue(string key)
    {
        if (TryGetStringValue(key, out var value))
            return value!;

        throw new Exception("Value could not be retrieved.");
    }

    /// <summary>
    /// Retrieves an integer value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <returns>An integer representing the value.</returns>
    /// <exception cref="Exception">Thrown when the value could not be found or is not a valid integer.</exception>
    public static int GetIntValue(string key)
    {
        if (TryGetIntValue(key, out var value))
            return value;

        throw new Exception("Value could not be retrieved.");
    }

    /// <summary>
    /// Retrieves a double value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <returns>A double representing the value.</returns>
    /// <exception cref="Exception">Thrown when the value could not be found or is not a valid double.</exception>
    public static double GetDoubleValue(string key)
    {
        if (TryGetDoubleValue(key, out var value))
            return value;

        throw new Exception("Value could not be retrieved.");
    }

    /// <summary>
    /// Retrieves a decimal value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <returns>A decimal representing the value.</returns>
    /// <exception cref="Exception">Thrown when the value could not be found or is not a valid decimal.</exception>
    public static decimal GetDecimalValue(string key)
    {
        if (TryGetDecimalValue(key, out var value))
            return value;

        throw new Exception("Value could not be retrieved.");
    }

    /// <summary>
    /// Retrieves a boolean value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <returns>A boolean representing the value.</returns>
    /// <exception cref="Exception">Thrown when the value could not be found or is not a valid boolean.</exception>
    public static bool GetBooleanValue(string key)
    {
        if (TryGetBooleanValue(key, out var value))
            return value;

        throw new Exception("Value could not be retrieved.");
    }

    /// <summary>
    /// Tries to retrieve a string value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <param name="value">When this method returns, contains the string value retrieved, or null if the retrieval failed.</param>
    /// <returns>True if the value was successfully retrieved; otherwise, false.</returns>
    public static bool TryGetStringValue(string key, out string? value)
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
    /// Tries to retrieve an integer value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <param name="value">When this method returns, contains the integer value retrieved, or 0 if the retrieval failed.</param>
    /// <returns>True if the value was successfully retrieved and parsed; otherwise, false.</returns>
    public static bool TryGetIntValue(string key, out int value)
    {
        var retrievedValue = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrEmpty(retrievedValue))
            return int.TryParse(retrievedValue, out value);

        value = 0;
        return false;
    }

    /// <summary>
    /// Tries to retrieve a double value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <param name="value">When this method returns, contains the double value retrieved, or 0.0 if the retrieval failed.</param>
    /// <returns>True if the value was successfully retrieved and parsed; otherwise, false.</returns>
    public static bool TryGetDoubleValue(string key, out double value)
    {
        var retrievedValue = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrEmpty(retrievedValue))
            return double.TryParse(retrievedValue, out value);

        value = 0.0;
        return false;
    }

    /// <summary>
    /// Tries to retrieve a decimal value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <param name="value">When this method returns, contains the decimal value retrieved, or 0.0m if the retrieval failed.</param>
    /// <returns>True if the value was successfully retrieved and parsed; otherwise, false.</returns>
    public static bool TryGetDecimalValue(string key, out decimal value)
    {
        var retrievedValue = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrEmpty(retrievedValue))
            return decimal.TryParse(retrievedValue, out value);

        value = 0.0m;
        return false;
    }

    /// <summary>
    /// Tries to retrieve a boolean value from the current environment.
    /// </summary>
    /// <param name="key">The key to retrieve the value via.</param>
    /// <param name="value">When this method returns, contains the boolean value retrieved, or false if the retrieval failed.</param>
    /// <returns>True if the value was successfully retrieved and parsed; otherwise, false.</returns>
    public static bool TryGetBooleanValue(string key, out bool value)
    {
        var retrievedValue = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrEmpty(retrievedValue))
            return bool.TryParse(retrievedValue, out value);

        value = false;
        return false;
    }

    /// <summary>
    /// Determines whether an environment key has a set value.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if a value is set; otherwise, false.</returns>
    public static bool HasValue(string key)
    {
        var retrievedValue = Environment.GetEnvironmentVariable(key);
        return !string.IsNullOrEmpty(retrievedValue);
    }
}