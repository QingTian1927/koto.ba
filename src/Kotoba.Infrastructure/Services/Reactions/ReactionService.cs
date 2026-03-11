using Kotoba.Core.Interfaces;
using Kotoba.Shared.DTOs;
using Kotoba.Infrastructure.Data;
using Kotoba.Domain.Entities;
using Kotoba.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Reactions
{ 
    public class ReactionService : IReactionService
    {
        private readonly ApplicationDbContext _context;
        public ReactionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ReactionDto?> AddOrUpdateReactionAsync(string userId, Guid messageId, ReactionType reactionType)
        {
            var existingMessage = await _context.Messages.AnyAsync(m => m.Id == messageId && !m.IsDeleted);
            if (!existingMessage) return null;
            var existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);
            if (existingReaction != null)
            {
                existingReaction.Type = reactionType;
                existingReaction.CreatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return new ReactionDto
                {
                    UserId = userId,
                    MessageId = messageId,
                    Type = reactionType,
                    CreatedAt = existingReaction.CreatedAt
                };
            }
            else
            {
                var reaction = new Reaction
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    UserId = userId,
                    Type = reactionType,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Reactions.Add(reaction);
                await _context.SaveChangesAsync();
                return new ReactionDto
                {
                    UserId = userId,
                    MessageId = messageId,
                    Type = reactionType,
                    CreatedAt = reaction.CreatedAt
                };
            }
        }
        public async Task<bool> RemoveReactionAsync(string userId, Guid messageId)
        {
            var reaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);
            if (reaction != null)
            {
                _context.Reactions.Remove(reaction);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<ReactionDto>> GetReactionsAsync(Guid messageId)
        {
            return await _context.Reactions
                .Where(r => r.MessageId == messageId)
                .Select(r => new ReactionDto
                {
                    UserId = r.UserId,
                    MessageId = r.MessageId,
                    Type = r.Type,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }
}
