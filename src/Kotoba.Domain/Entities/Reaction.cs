using Kotoba.Domain.Enums;

namespace Kotoba.Domain.Entities;

public class Reaction
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Message Message { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

