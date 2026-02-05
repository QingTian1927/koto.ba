namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for managing user's current thought (one per user, auto-expires)
/// Owner: Hoàn (AI & Social Features)
/// </summary>
public interface ICurrentThoughtService
{
    Task<bool> SetThoughtAsync(string userId, string content);
    Task<string?> GetThoughtAsync(string userId);
}

