using Kotoba.Shared.DTOs;

namespace Kotoba.Core.Interfaces;

/// <summary>
/// Service for generating AI reply suggestions
/// Owner: HoÃ n (AI & Social Features)
/// </summary>
public interface IAIReplyService
{
    Task<List<string>> GenerateSuggestionsAsync(AIReplyRequest request);
}
