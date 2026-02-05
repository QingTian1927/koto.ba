namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for managing typing indicators
/// Owner: Hoàn (Realtime Interaction & Typing)
/// </summary>
public interface ITypingService
{
    Task SetTypingAsync(string userId, Guid conversationId, bool isTyping);
}

