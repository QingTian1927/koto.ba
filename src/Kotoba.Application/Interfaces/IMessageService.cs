using Kotoba.Application.DTOs;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for sending and retrieving messages
/// Owner: Vinh (Message Persistence & History)
/// </summary>
public interface IMessageService
{
    Task<MessageDto?> SendMessageAsync(SendMessageRequest request);
    Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, PagingRequest paging);
}

