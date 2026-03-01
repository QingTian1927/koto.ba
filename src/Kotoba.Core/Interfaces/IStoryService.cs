using Kotoba.Domain.DTOs;

namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for managing user stories (24-hour expiration)
/// Owner: HoÃ n (AI & Social Features)
/// </summary>
public interface IStoryService
{
    Task<StoryDto?> CreateStoryAsync(CreateStoryRequest request);
    Task<List<StoryDto>> GetActiveStoriesAsync();
}
