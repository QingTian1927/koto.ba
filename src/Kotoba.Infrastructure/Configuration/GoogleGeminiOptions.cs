namespace Kotoba.Infrastructure.Configuration;

/// <summary>
/// Configuration options for Google Gemini AI service
/// </summary>
public class GoogleGeminiOptions
{
    public const string SectionName = "GoogleGemini";

    /// <summary>
    /// Google Gemini API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Ordered list of models to try (fallback chain)
    /// </summary>
    public List<GeminiModelConfig> FallbackChain { get; set; } = new();
}

/// <summary>
/// Configuration for individual Gemini model
/// </summary>
public class GeminiModelConfig
{
    /// <summary>
    /// Model identifier (e.g., "gemini-1.5-pro")
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// Maximum tokens to generate
    /// </summary>
    public int MaxTokens { get; set; } = 150;
}
