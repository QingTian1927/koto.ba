using Kotoba.Application.DTOs;

namespace Kotoba.Application.Interfaces;

/// <summary>
/// Service for generating AI reply suggestions
/// Owner: Hoàn (AI & Social Features)
/// </summary>
public interface IAIReplyService
{
    Task<List<string>> GenerateSuggestionsAsync(AIReplyRequest request);
}

