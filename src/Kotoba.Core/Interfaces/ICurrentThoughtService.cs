namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for managing user's current thought (one per user, auto-expires)
/// Owner: HoÃ n (AI & Social Features)
/// </summary>
public interface ICurrentThoughtService
{
    Task<bool> SetThoughtAsync(string userId, string content);
    Task<string?> GetThoughtAsync(string userId);
}
