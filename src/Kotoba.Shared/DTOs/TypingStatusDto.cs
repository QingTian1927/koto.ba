namespace Kotoba.Shared.DTOs;

public class TypingStatusDto
{
    public string UserId { get; set; } = string.Empty;
    public Guid ConversationId { get; set; }
    public bool IsTyping { get; set; }
}

