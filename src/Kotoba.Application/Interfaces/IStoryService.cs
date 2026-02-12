using Kotoba.Shared.DTOs;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for managing user stories (24-hour expiration)
/// Owner: Hoàn (AI & Social Features)
/// </summary>
public interface IStoryService
{
    Task<StoryDto?> CreateStoryAsync(CreateStoryRequest request);
    Task<List<StoryDto>> GetActiveStoriesAsync();
}

