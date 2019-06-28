namespace dotenv.net.Interfaces
{
    public interface IEnvReader
    {
        string GetValue(string key);

        T GetValue<T>(string key);
        
        bool TryGetValue(string key, out string value);

        T TryGetValue<T>(string key, out T value);
    }
}