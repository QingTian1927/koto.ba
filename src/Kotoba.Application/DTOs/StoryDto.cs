namespace Kotoba.Application.DTOs;

public class StoryDto
{
    public Guid StoryId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public DateTime ExpiresAt { get; set; }
}

