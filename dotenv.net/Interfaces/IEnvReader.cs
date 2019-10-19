namespace dotenv.net.Interfaces
{
    public interface IEnvReader
    {
        string GetValue(string key);

        T GetValue<T>(string key) where T : struct;
        
        bool TryGetValue(string key, out string value);

        bool TryGetValue<T>(string key, out T value) where T : struct;
    }
}