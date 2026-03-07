using Kotoba.Core.Interfaces;
using Kotoba.Shared.DTOs;
using Kotoba.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Messages;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _context;

    public MessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MessageDto?> SendMessageAsync(SendMessageRequest request)
    {
        // TODO: Implement send message
        throw new NotImplementedException();
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, PagingRequest paging)
    {
        // TODO: Implement get messages
        throw new NotImplementedException();
    }
}
