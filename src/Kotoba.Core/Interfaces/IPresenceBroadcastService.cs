using Kotoba.Shared.DTOs;

namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for broadcasting user presence status changes
/// Owner: Dũng (Identity & User Management)
/// </summary>
public interface IPresenceBroadcastService
{
    /// <summary>
    /// Get all currently online users
    /// </summary>
    Task<List<PresenceUpdateDto>> GetAllOnlineUsersAsync();

    /// <summary>
    /// Broadcast when a user comes online
    /// </summary>
    Task<PresenceUpdateDto> NotifyUserOnlineAsync(string userId);

    /// <summary>
    /// Broadcast when a user goes offline
    /// </summary>
    Task<PresenceUpdateDto> NotifyUserOfflineAsync(string userId);
}
