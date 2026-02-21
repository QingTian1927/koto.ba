using Kotoba.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotoba.Application.Interfaces.Repositories
{ 
    public interface IMessageRepository
    {
        /// <summary>Persists a new message and returns it with generated Id/CreatedAt.</summary>
        Task<Message> AddAsync(Message message);

        /// <summary>Returns all non-deleted messages for a conversation, ordered oldest → newest.</summary>
        Task<List<Message>> GetByConversationIdAsync(Guid conversationId);
    }
}
