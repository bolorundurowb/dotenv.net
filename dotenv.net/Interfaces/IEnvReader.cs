namespace dotenv.net.Interfaces
{
    public interface IEnvReader
    {
        string GetStringValue(string key);

        int GetIntValue(string key);

        double GetDoubleValue(string key);

        decimal GetDecimalValue(string key);

        bool TryGetStringValue(string key, out string value);

        bool TryGetIntValue(string key, out int value);

        bool TryGetDoubleValue(string key, out double value);

        bool TryGetDecimalValue(string key, out decimal value);
    }
}