using Kotoba.Core.Interfaces;
using Kotoba.Domain.DTOs;
using Kotoba.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Conversations;

public class ConversationService : IConversationService
{
    private readonly ApplicationDbContext _context;

    public ConversationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConversationDto?> CreateDirectConversationAsync(string userAId, string userBId)
    {
        // TODO: Implement create direct conversation
        throw new NotImplementedException();
    }

    public async Task<ConversationDto?> CreateGroupConversationAsync(CreateGroupRequest request)
    {
        // TODO: Implement create group conversation
        throw new NotImplementedException();
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(string userId)
    {
        // TODO: Implement get user conversations
        throw new NotImplementedException();
    }

    public async Task<ConversationDto?> GetConversationDetailAsync(Guid conversationId)
    {
        // TODO: Implement get conversation detail
        throw new NotImplementedException();
    }
}
