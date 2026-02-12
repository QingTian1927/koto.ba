using Kotoba.Application.Interfaces;
using Kotoba.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Kotoba.Server.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationsController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<ConversationsResponse>> GetUserConversations(string userId)
    {
        var result = await _conversationService.GetUserConversationsAsync(userId);

        var response = new ConversationsResponse
        {
            Conversations = result.Conversations,
            Messages = result.Messages
        };

        return Ok(response);
    }

    [HttpPost("{conversationId:guid}/messages")]
    public async Task<IActionResult> AddMessage(Guid conversationId, [FromBody] SendMessageRequest request)
    {
        if (conversationId == Guid.Empty)
        {
            return BadRequest("Conversation id is required.");
        }

        await _conversationService.AddMessage(conversationId.ToString(), request.Content);
        return NoContent();
    }
}
