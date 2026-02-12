using Kotoba.Shared.DTOs;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for managing user accounts and profiles
/// Owner: Dũng (Identity & User Management)
/// </summary>
public interface IUserService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<bool> LoginAsync(LoginRequest request);
    Task<UserProfile?> GetUserProfileAsync(string userId);
    Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileRequest request);
}

