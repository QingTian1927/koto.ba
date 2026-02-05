using Kotoba.Application.DTOs;
using Kotoba.Domain.Enums;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for managing message reactions
/// Owner: Vinh (Reactions & Attachments)
/// </summary>
public interface IReactionService
{
    Task<ReactionDto?> AddOrUpdateReactionAsync(string userId, Guid messageId, ReactionType reactionType);
    Task<bool> RemoveReactionAsync(string userId, Guid messageId);
    Task<List<ReactionDto>> GetReactionsAsync(Guid messageId);
}

