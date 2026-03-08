using Kotoba.Core.Interfaces;
using Kotoba.Shared.DTOs;

namespace Kotoba.Infrastructure.Services.Identity;

public class PresenceBroadcastService : IPresenceBroadcastService
{
    private readonly IPresenceService _presenceService;
    private readonly IUserService _userService;

    public PresenceBroadcastService(IPresenceService presenceService, IUserService userService)
    {
        _presenceService = presenceService;
        _userService = userService;
    }

    public async Task<List<PresenceUpdateDto>> GetAllOnlineUsersAsync()
    {
        var onlineUserIds = await _presenceService.GetAllOnlineUsersAsync();
        var result = new List<PresenceUpdateDto>();

        foreach (var userId in onlineUserIds)
        {
            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile != null)
            {
                result.Add(new PresenceUpdateDto
                {
                    UserId = profile.UserId,
                    DisplayName = profile.DisplayName,
                    IsOnline = true,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        return result;
    }

    public async Task<PresenceUpdateDto> NotifyUserOnlineAsync(string userId)
    {
        await _presenceService.SetOnlineAsync(userId);

        var profile = await _userService.GetUserProfileAsync(userId);
        if (profile == null)
        {
            throw new InvalidOperationException($"User with id {userId} not found");
        }

        return new PresenceUpdateDto
        {
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            IsOnline = true,
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task<PresenceUpdateDto> NotifyUserOfflineAsync(string userId)
    {
        await _presenceService.SetOfflineAsync(userId);

        var profile = await _userService.GetUserProfileAsync(userId);
        if (profile == null)
        {
            throw new InvalidOperationException($"User with id {userId} not found");
        }

        return new PresenceUpdateDto
        {
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            IsOnline = false,
            Timestamp = DateTime.UtcNow
        };
    }
}
