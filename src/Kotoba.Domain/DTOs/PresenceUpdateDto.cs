namespace Kotoba.Domain.DTOs;

public class PresenceUpdateDto
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
