using Kotoba.Core.Interfaces;
using Kotoba.Domain.Enums;
using Kotoba.Infrastructure.Configuration;
using Kotoba.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Kotoba.Infrastructure.Services.Social;

/// <summary>
/// AI Reply service with Google Gemini fallback chain
/// Owner: Dũng (AI & Social Features)
/// </summary>
public class AIReplyService : IAIReplyService
{
    private readonly GoogleGeminiOptions _options;
    private readonly ILogger<AIReplyService> _logger;
    private readonly HttpClient _httpClient;
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 1000;

    public AIReplyService(
        IOptions<GoogleGeminiOptions> options,
        ILogger<AIReplyService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<string>> GenerateSuggestionsAsync(AIReplyRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "Starting AI suggestion generation for user {UserId} with tone {Tone}",
            request.UserId, request.Tone);

        // Validate model list
        if (_options.FallbackChain == null || _options.FallbackChain.Count == 0)
        {
            _logger.LogError("Model fallback chain is empty. Cannot generate suggestions.");
            throw new InvalidOperationException("AI model configuration is empty.");
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogError("Google Gemini API key is not configured.");
            throw new InvalidOperationException("AI service is not configured.");
        }

        // Try each model in the fallback chain
        Exception? lastException = null;
        for (int modelIndex = 0; modelIndex < _options.FallbackChain.Count; modelIndex++)
        {
            var modelConfig = _options.FallbackChain[modelIndex];
            _logger.LogInformation(
                "Attempting model {ModelName} (position {Position}/{Total})",
                modelConfig.ModelName, modelIndex + 1, _options.FallbackChain.Count);

            try
            {
                var suggestions = await TryGenerateWithModelAsync(request, modelConfig);

                stopwatch.Stop();
                _logger.LogInformation(
                    "✅ Successfully generated suggestions using {ModelName} in {ElapsedMs}ms",
                    modelConfig.ModelName, stopwatch.ElapsedMilliseconds);

                return suggestions;
            }
            catch (Exception ex)
            {
                lastException = ex;
                var shouldFallback = ShouldFallbackToNextModel(ex);

                _logger.LogWarning(ex,
                    "❌ Model {ModelName} failed: {ErrorMessage}. Fallback: {WillFallback}",
                    modelConfig.ModelName, ex.Message, shouldFallback);

                if (!shouldFallback)
                {
                    // Fatal error - don't try other models
                    throw;
                }

                // Continue to next model in chain
            }
        }

        // All models exhausted
        _logger.LogError(lastException,
            "All models in fallback chain exhausted. Total models tried: {Count}",
            _options.FallbackChain.Count);

        throw new InvalidOperationException(
            $"Failed to generate AI suggestions after trying {_options.FallbackChain.Count} models.",
            lastException);
    }

    private async Task<List<string>> TryGenerateWithModelAsync(
        AIReplyRequest request,
        GeminiModelConfig modelConfig)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                _logger.LogDebug("Attempt {Attempt}/{MaxRetries} for model {ModelName}",
                    attempt, MaxRetries, modelConfig.ModelName);

                return await CallGeminiApiAsync(request, modelConfig);
            }
            catch (Exception ex)
            {
                lastException = ex;
                var shouldRetry = ShouldRetry(ex);

                if (!shouldRetry || attempt >= MaxRetries)
                {
                    throw; // Fatal or max retries reached
                }

                _logger.LogDebug(
                    "Retrying in {DelayMs}ms (attempt {Attempt}/{MaxRetries})",
                    RetryDelayMs, attempt, MaxRetries);

                await Task.Delay(RetryDelayMs);
            }
        }

        throw lastException!;
    }

    private async Task<List<string>> CallGeminiApiAsync(
        AIReplyRequest request,
        GeminiModelConfig modelConfig)
    {
        var systemPrompt = GetSystemPromptForTone(request.Tone);
        var userPrompt = $@"Original message: ""{request.OriginalMessage}""

Please generate exactly 3 different reply suggestions to the above message. Each suggestion should be on a new line and numbered (1., 2., 3.).";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = systemPrompt + "\n\n" + userPrompt }
                    }
                }
            },
            generationConfig = new
            {
                maxOutputTokens = modelConfig.MaxTokens,
                temperature = GetTemperatureForTone(request.Tone)
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelConfig.ModelName}:generateContent?key={_options.ApiKey}";

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, httpContent);

        // Check for specific HTTP status codes
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Invalid API key"),
                HttpStatusCode.BadRequest => new ArgumentException($"Bad request: {errorContent}"),
                HttpStatusCode.TooManyRequests => new InvalidOperationException("Quota exceeded"),
                HttpStatusCode.ServiceUnavailable => new InvalidOperationException("Service unavailable"),
                _ => new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}")
            };
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

        if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Count == 0)
        {
            throw new InvalidOperationException("No response from Gemini API");
        }

        var generatedText = geminiResponse.Candidates[0]?.Content?.Parts?[0]?.Text;
        if (string.IsNullOrWhiteSpace(generatedText))
        {
            throw new InvalidOperationException("Empty response from Gemini API");
        }

        return ParseSuggestions(generatedText);
    }

    private string GetSystemPromptForTone(AITone tone)
    {
        return tone switch
        {
            AITone.Polite => "You are a courteous and formal communication assistant. Generate polite, respectful replies that are appropriate for professional or formal contexts. Use respectful language and maintain a professional tone.",
            AITone.Friendly => "You are a warm and casual communication assistant. Generate friendly, approachable replies that feel natural in casual conversations. Use a conversational tone and add warmth to the messages.",
            AITone.Confident => "You are an assertive and direct communication assistant. Generate confident, clear replies that communicate decisively. Be direct and authoritative while remaining respectful.",
            _ => "You are a helpful communication assistant. Generate appropriate reply suggestions."
        };
    }

    private double GetTemperatureForTone(AITone tone)
    {
        return tone switch
        {
            AITone.Polite => 0.3,      // More conservative
            AITone.Friendly => 0.7,    // More creative
            AITone.Confident => 0.5,   // Balanced
            _ => 0.5
        };
    }

    private List<string> ParseSuggestions(string generatedText)
    {
        var suggestions = new List<string>();
        var lines = generatedText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            // Remove numbering like "1.", "2.", "*", "-", etc.
            var cleaned = System.Text.RegularExpressions.Regex.Replace(
                trimmed, @"^[\d\*\-•]+[\.\)]\s*", "");

            if (!string.IsNullOrWhiteSpace(cleaned))
            {
                suggestions.Add(cleaned);
            }

            if (suggestions.Count >= 3)
            {
                break;
            }
        }

        // Ensure we always return 3 suggestions
        while (suggestions.Count < 3)
        {
            suggestions.Add($"Thank you for your message. (Suggestion {suggestions.Count + 1})");
        }

        return suggestions.Take(3).ToList();
    }

    private bool ShouldFallbackToNextModel(Exception ex)
    {
        return ex switch
        {
            // Immediate fallback - these errors won't improve with same model
            UnauthorizedAccessException => false,  // Bad API key - won't work on any model
            ArgumentException => false,            // Bad request - same for all models
            InvalidOperationException ioe when ioe.Message.Contains("Quota exceeded") => true,
            InvalidOperationException ioe when ioe.Message.Contains("Service unavailable") => true,
            HttpRequestException => true,          // Network/availability issues
            TaskCanceledException => true,         // Timeout
            _ => true                              // Unknown error - try fallback
        };
    }

    private bool ShouldRetry(Exception ex)
    {
        return ex switch
        {
            // Immediate failures - don't retry
            UnauthorizedAccessException => false,
            ArgumentException => false,
            InvalidOperationException ioe when ioe.Message.Contains("Quota exceeded") => false,

            // Retryable errors
            HttpRequestException => true,
            TaskCanceledException => true,
            InvalidOperationException ioe when ioe.Message.Contains("Service unavailable") => true,
            _ => false
        };
    }

    #region Gemini API Response Models

    private class GeminiResponse
    {
        public List<Candidate>? Candidates { get; set; }
    }

    private class Candidate
    {
        public Content? Content { get; set; }
    }

    private class Content
    {
        public List<Part>? Parts { get; set; }
    }

    private class Part
    {
        public string? Text { get; set; }
    }

    #endregion
}
