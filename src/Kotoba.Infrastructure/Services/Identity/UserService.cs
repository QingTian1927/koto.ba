using Kotoba.Core.Interfaces;
using Kotoba.Shared.DTOs;
using Kotoba.Domain.Entities;
using Kotoba.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserService(
        ApplicationDbContext context,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        return existingUser == null;
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        // Validate email/password exist
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || string.IsNullOrEmpty(request.Password))
            return false;
        return true;
    }

    public async Task<UserProfile?> GetUserProfileAsync(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return null;

        return new UserProfile
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            IsOnline = user.IsOnline,
            LastSeenAt = user.LastSeenAt
        };
    }

    public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return false;

        user.DisplayName = request.DisplayName;
        user.AvatarUrl = request.AvatarUrl;
        await _context.SaveChangesAsync();
        return true;
    }
}
