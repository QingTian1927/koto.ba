using Kotoba.Shared.DTOs;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for managing user online/offline presence
/// Owner: Dũng (Identity & User Management)
/// </summary>
public interface IPresenceService
{
    Task SetOnlineAsync(string userId);
    Task SetOfflineAsync(string userId);
    Task<bool> GetUserPresenceAsync(string userId);
}

