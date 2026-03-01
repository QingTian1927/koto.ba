using Kotoba.Core.Interfaces;
using Kotoba.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Kotoba.Infrastructure.Services.Identity;

public class PresenceService : IPresenceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public PresenceService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task SetOnlineAsync(string userId)
    {
        // TODO: Implement set user online
        throw new NotImplementedException();
    }

    public async Task SetOfflineAsync(string userId)
    {
        // TODO: Implement set user offline
        throw new NotImplementedException();
    }

    public async Task<bool> GetUserPresenceAsync(string userId)
    {
        // TODO: Implement get user presence
        throw new NotImplementedException();
    }
}
