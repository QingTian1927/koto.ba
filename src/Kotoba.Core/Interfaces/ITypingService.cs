namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for managing typing indicators
/// Owner: HoÃ n (Realtime Interaction & Typing)
/// </summary>
public interface ITypingService
{
    Task SetTypingAsync(string userId, Guid conversationId, bool isTyping);
}
