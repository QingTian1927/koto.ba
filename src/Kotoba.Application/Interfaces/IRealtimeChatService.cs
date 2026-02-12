using Kotoba.Shared.DTOs;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for broadcasting realtime events via SignalR
/// Owner: Hoàn (Realtime Interaction & Typing)
/// </summary>
public interface IRealtimeChatService
{
    Task BroadcastMessageAsync(MessageDto message);
    Task BroadcastTypingAsync(TypingStatusDto typingStatus);
}

