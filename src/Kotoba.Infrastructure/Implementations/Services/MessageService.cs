using Kotoba.Application.DTOs;
using Kotoba.Application.Interfaces;
using Kotoba.Domain.Entities;
using Kotoba.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Service for sending and retrieving messages
/// Owner: Vinh (Message Persistence & History)
/// </summary>
/// 
namespace Kotoba.Infrastructure.Implementations.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MessageDto?> SendMessageAsync(SendMessageRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return null;
            }

            var isMember = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == request.ConversationId
                             && cp.UserId == request.SenderId
                             && cp.IsActive); 

            if (!isMember)
            {
                return null; 
            }
            var message = new Message
            {
                Id = Guid.NewGuid(), 
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return new MessageDto
            {
                MessageId = message.Id, 
                ConversationId = request.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content,
                CreatedAt = message.CreatedAt 
            };
        }

        public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, PagingRequest paging)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .Select(m => new MessageDto
                {
                    MessageId = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
            return messages;
        }
    }
}
