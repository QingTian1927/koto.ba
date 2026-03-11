using Kotoba.Domain.Enums;

namespace Kotoba.Shared.DTOs;

public class MessageDto
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public MessageStatus? Status { get; set; }
    public List<ReactionDto> Reactions { get; set; } = new();
    public List<AttachmentDto> Attachments { get; set; } = new();
}

}
