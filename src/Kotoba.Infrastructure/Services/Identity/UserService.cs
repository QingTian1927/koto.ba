using Kotoba.Core.Interfaces;
using Kotoba.Domain.DTOs;
using Kotoba.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Kotoba.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public UserService(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        // TODO: Implement user registration
        throw new NotImplementedException();
    }

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        // TODO: Implement user login
        throw new NotImplementedException();
    }

    public async Task<UserProfile?> GetUserProfileAsync(string userId)
    {
        // TODO: Implement get user profile
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
    {
        // TODO: Implement update user profile
        throw new NotImplementedException();
    }
}
