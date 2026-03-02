using Kotoba.Core.Interfaces;
using Kotoba.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Kotoba.Infrastructure.Services.Identity;

public class PresenceService : IPresenceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string PresenceKeyPrefix = "presence:";
    private const int PresenceCacheDurationMinutes = 30;

    public PresenceService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task SetOnlineAsync(string userId)
    {
        var cacheKey = $"{PresenceKeyPrefix}{userId}";
        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(PresenceCacheDurationMinutes)
        };

        _cache.Set(cacheKey, true, cacheOptions);
        await Task.CompletedTask;
    }

    public async Task SetOfflineAsync(string userId)
    {
        var cacheKey = $"{PresenceKeyPrefix}{userId}";
        _cache.Remove(cacheKey);
        await Task.CompletedTask;
    }

    public async Task<bool> GetUserPresenceAsync(string userId)
    {
        var cacheKey = $"{PresenceKeyPrefix}{userId}";
        var isOnline = _cache.TryGetValue(cacheKey, out _);
        return await Task.FromResult(isOnline);
    }
}
