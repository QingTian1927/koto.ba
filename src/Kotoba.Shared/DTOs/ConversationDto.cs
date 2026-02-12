using Kotoba.Domain.Enums;

namespace Kotoba.Shared.DTOs;

public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public ConversationType Type { get; set; }
    public string? GroupName { get; set; }
    public List<string> ParticipantIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

