using System;

namespace Caching
{
    public interface ICache
    {
        void SetValue(string key, object value, DateTimeOffset expiration);

        void SetValue(string key, object value, int secondsToKeep);

        T GetValue<T>(string key);

        void Delete(string key);

        void Dispose();
    }
}
