using StackExchange.Redis;

namespace SocialMedia.Infrastructure;

public class GarnetDistributedCache : IDistributedCache
{
    private readonly IDatabase _db;
    private readonly string _instanceName;

    public GarnetDistributedCache(IConnectionMultiplexer redis, string instanceName = "")
    {
        _db = redis.GetDatabase();
        _instanceName = instanceName;
    }

    private string GetKey(string key) => _instanceName + key;

    // --- ASYNC METHODS ---

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        var value = await _db.StringGetAsync(GetKey(key));
        return value.IsNull ? null : (byte[]?)value;
    }

    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        // Calculate expiration: Garnet supports absolute and relative expiration via standard SET
        TimeSpan? expiry = null;
        if (options.AbsoluteExpirationRelativeToNow.HasValue)
            expiry = options.AbsoluteExpirationRelativeToNow;
        else if (options.AbsoluteExpiration.HasValue)
            expiry = options.AbsoluteExpiration.Value - DateTimeOffset.Now;
        else if (options.SlidingExpiration.HasValue)
            expiry = options.SlidingExpiration;

        // Uses standard SET [key] [value] EX [seconds] which Garnet supports without Lua
        await _db.StringSetAsync(GetKey(key), value, expiry);
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        // For simple caching, we just touch the key to reset TTL if it exists
        return _db.KeyIdleTimeAsync(GetKey(key));
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        return _db.KeyDeleteAsync(GetKey(key));
    }

    // --- SYNC METHODS ---

    public byte[]? Get(string key)
    {
        var value = _db.StringGet(GetKey(key));
        return value.IsNull ? null : (byte[]?)value;
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        TimeSpan? expiry = options.AbsoluteExpirationRelativeToNow ?? options.SlidingExpiration;
        _db.StringSet(GetKey(key), value, expiry);
    }

    public void Refresh(string key) => _db.KeyIdleTime(GetKey(key));

    public void Remove(string key) => _db.KeyDelete(GetKey(key));
}