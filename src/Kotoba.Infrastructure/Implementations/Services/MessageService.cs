using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kotoba.Application.Interfaces;
using Kotoba.Domain.Entities;
using Kotoba.Shared.DTOs;
using Kotoba.Application.Interfaces.Repositories;

namespace Kotoba.Infrastructure.Implementations.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<MessageDto?> SendMessageAsync(SendMessageRequest request)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _messageRepository.AddAsync(message);
            return MapToDto(saved);
        }

        public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, PagingRequest paging)
        {
            var messages = await _messageRepository.GetByConversationIdAsync(conversationId);
            return messages.Select(MapToDto).ToList();
        }

        private static MessageDto MapToDto(Message m) => new()
        {
            MessageId = m.Id,
            ConversationId = m.ConversationId,
            SenderId = m.SenderId,
            Content = m.Content,
            CreatedAt = m.CreatedAt
        };
    }
}
