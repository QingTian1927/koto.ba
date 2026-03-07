using Kotoba.Shared.DTOs;

namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for broadcasting realtime events via SignalR
/// Owner: HoÃ n (Realtime Interaction & Typing)
/// </summary>
public interface IRealtimeChatService
{
    Task BroadcastMessageAsync(MessageDto message);
    Task BroadcastTypingAsync(TypingStatusDto typingStatus);
}
