using Kotoba.Core.Interfaces;
using Kotoba.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kotoba.Server.Controllers;

/// <summary>
/// AI suggestion endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIReplyService _aiService;
    private readonly ILogger<AIController> _logger;

    public AIController(
        IAIReplyService aiService,
        ILogger<AIController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Generate AI reply suggestions
    /// </summary>
    /// <param name="request">AI reply request with original message and tone</param>
    /// <returns>List of 3 suggestion strings</returns>
    [HttpPost("suggestions")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<string>>> GetSuggestions([FromBody] AIReplyRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.OriginalMessage))
            {
                return BadRequest("Original message is required.");
            }

            // Get authenticated user ID
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated.");
            }

            // Override request userId with authenticated user for security
            request.UserId = userId;

            _logger.LogInformation(
                "Processing AI suggestion request for user {UserId} with tone {Tone}",
                userId, request.Tone);

            var suggestions = await _aiService.GenerateSuggestionsAsync(request);

            return Ok(suggestions);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for AI suggestions");
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "AI service authentication failed");
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                "AI service is temporarily unavailable.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "AI service operation failed");
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in AI suggestions");
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error occurred while generating suggestions.");
        }
    }
}
