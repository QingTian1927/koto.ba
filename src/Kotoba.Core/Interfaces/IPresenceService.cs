using Kotoba.Domain.DTOs;

namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for managing user online/offline presence
/// Owner: DÅ©ng (Identity & User Management)
/// </summary>
public interface IPresenceService
{
    Task SetOnlineAsync(string userId);
    Task SetOfflineAsync(string userId);
    Task<bool> GetUserPresenceAsync(string userId);
}
