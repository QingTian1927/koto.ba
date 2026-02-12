using Kotoba.Domain.Enums;

namespace Kotoba.Shared.DTOs;

public class ReactionDto
{
    public Guid MessageId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}

