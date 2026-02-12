namespace Kotoba.Shared.DTOs;

public class UpdateProfileRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

