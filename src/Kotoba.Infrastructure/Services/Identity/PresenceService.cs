using Kotoba.Core.Interfaces;
using Kotoba.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Kotoba.Infrastructure.Services.Identity;

public class PresenceService : IPresenceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string PresenceKeyPrefix = "presence:";
    private const string OnlineUsersKey = "online_users_list";
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

        // Add to online users list
        var onlineUsers = _cache.Get<List<string>>(OnlineUsersKey) ?? new List<string>();

        if (!onlineUsers.Contains(userId))
        {
            onlineUsers.Add(userId);
            _cache.Set(OnlineUsersKey, onlineUsers);
        }

        await Task.CompletedTask;
    }

    public async Task SetOfflineAsync(string userId)
    {
        var cacheKey = $"{PresenceKeyPrefix}{userId}";
        _cache.Remove(cacheKey);

        // Remove from online users list
        var onlineUsers = _cache.Get<List<string>>(OnlineUsersKey);
        if (onlineUsers != null)
        {
            onlineUsers.Remove(userId);
            _cache.Set(OnlineUsersKey, onlineUsers);
        }

        await Task.CompletedTask;
    }

    public async Task<bool> GetUserPresenceAsync(string userId)
    {
        var cacheKey = $"{PresenceKeyPrefix}{userId}";
        var isOnline = _cache.TryGetValue(cacheKey, out _);
        return await Task.FromResult(isOnline);
    }

    public async Task<List<string>> GetAllOnlineUsersAsync()
    {
        var onlineUsers = _cache.Get<List<string>>(OnlineUsersKey) ?? new List<string>();
        return await Task.FromResult(onlineUsers);
    }
}
