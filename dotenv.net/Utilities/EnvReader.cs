using System;
using dotenv.net.Interfaces;

namespace dotenv.net.Utilities
{
    public class EnvReader : IEnvReader
    {
        public string GetValue(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        public T GetValue<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out string value)
        {
            throw new System.NotImplementedException();
        }

        public T TryGetValue<T>(string key, out T value)
        {
            throw new System.NotImplementedException();
        }
    }
}