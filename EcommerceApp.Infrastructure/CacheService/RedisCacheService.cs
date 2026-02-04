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
        try
        {
            var data = await distributedCache.GetStringAsync(key);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }
        catch (Exception)
        {
            // Redis is down or unreachable → fail silently
            return default;
        }
    }

    public async Task RemoveData(string key)
    {
        try
        {
            await distributedCache.RemoveAsync(key);
        }
        catch (Exception)
        {
            // ignore failures
        }
    }

    public async Task SetData<T>(string key, T data, TimeSpan? expiration = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            var jsonData = JsonSerializer.Serialize(data);
            await distributedCache.SetStringAsync(key, jsonData, options);
        }
        catch (Exception)
        {
            // ignore failures
        }
    }

    }
}
