using System;
using System.Collections.Generic;

namespace Asv.Tools
{
    public interface IConfiguration:IDisposable
    {
        IEnumerable<string> AvalableParts { get; }
        bool Exist<TPocoType>(string key);
        TPocoType Get<TPocoType>(string key, TPocoType defaultValue);
        void Set<TPocoType>(string key, TPocoType value);
        void Remove(string key);
    }

    public static class ConfigurationExtentions
    {
        public static TPocoType Get<TPocoType>(this IConfiguration src,string key) where TPocoType : new()
        {
            var defaultValue = new TPocoType();
            return src.Get(key, defaultValue);
        }

        public static void Update<TPocoType>(this IConfiguration src, Action<TPocoType> updateCallback) where TPocoType : new()
        {
            var value = src.Get<TPocoType>();
            updateCallback(value);
            src.Set(value);
        }

        public static TPocoType Get<TPocoType>(this IConfiguration src)
            where TPocoType :  new()
        {
            var defaultValue = new TPocoType();
            var value = src.Get(typeof(TPocoType).Name, defaultValue);
            return value;
        }

        public static void Set<TPocoType>(this IConfiguration src, TPocoType value)
            where TPocoType : new()
        {
            src.Set(typeof(TPocoType).Name, value);
        }

        public static void Remove<TPocoType>(this IConfiguration src)
            where TPocoType :  new()
        {
            src.Remove(typeof(TPocoType).Name);
        }

    }

    
}
