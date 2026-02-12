namespace Kotoba.Shared.DTOs;

public class ConversationsResponse
{
    public List<ConversationDto> Conversations { get; set; } = new();
    public List<MessageDto> Messages { get; set; } = new();
}
