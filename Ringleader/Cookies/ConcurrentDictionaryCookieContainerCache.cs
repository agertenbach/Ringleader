using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ringleader.Cookies
{
    public class ConcurrentDictionaryCookieContainerCache : ICookieContainerCache
    {
        private readonly ConcurrentDictionary<string, CookieContainer> _dictionary = new ConcurrentDictionary<string, CookieContainer>();

        private static string CompoundKey(string clientName, string key)
            => clientName + ":" + key;


        public Task<CookieContainer> GetOrAdd(string clientName, string key, CancellationToken token = default)
        {
            var c = _dictionary.GetOrAdd(CompoundKey(clientName, key), new CookieContainer());
            var copy = c.Clone();
            return Task.FromResult(copy);
        }

        public Task AddOrUpdate(string clientName, string key, CookieContainer value, CancellationToken token = default)
        {
            var copy = value.Clone();
            _dictionary.AddOrUpdate(CompoundKey(clientName, key), copy, (k, v) => copy);
            return Task.CompletedTask;
        }

        public Task Reset(string clientName, string key, CancellationToken token = default)
        {
            _dictionary.TryRemove(CompoundKey(clientName, key), out _);
            return Task.CompletedTask;
        }
    }
}
