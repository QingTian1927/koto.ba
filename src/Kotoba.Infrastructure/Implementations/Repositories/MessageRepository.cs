using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kotoba.Application.Interfaces.Repositories;
using Kotoba.Infrastructure.Data;
using Kotoba.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Implementations.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MessageRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Message> AddAsync(Message message)
        {
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId)
        {
            return await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
