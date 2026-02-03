using EcommerceApp.Application.Interfaces.CacheServices;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace EcommerceApp.Infrastructure.CacheService
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache distributedCache;
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

        public RedisCacheService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task<T?> GetData<T>(string key)
        {
            var data = await distributedCache.GetStringAsync(key);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }

        public async Task RemoveData(string key)
        {
            await distributedCache.RemoveAsync(key);
        }

        public async Task SetData<T>(string key, T data, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            var jsonData = JsonSerializer.Serialize(data);
            await distributedCache.SetStringAsync(key, jsonData, options);
        }
    }
}
