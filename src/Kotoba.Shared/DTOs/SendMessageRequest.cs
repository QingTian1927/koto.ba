namespace Kotoba.Shared.DTOs;

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

