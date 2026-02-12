using Kotoba.Domain.Enums;

namespace Kotoba.Shared.DTOs;

public class AIReplyRequest
{
    public string UserId { get; set; } = string.Empty;
    public string OriginalMessage { get; set; } = string.Empty;
    public AITone Tone { get; set; } = AITone.Friendly;
}

