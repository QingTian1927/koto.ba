using Kotoba.Application.Interfaces;
using Kotoba.Infrastructure.Data;
using Kotoba.Server.Hubs;
using Kotoba.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Kotoba.Server.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationsController : ControllerBase
{
    private readonly IConversationService _conversationService;
    
    private readonly IHubContext<ChatHub> _hubContext;

    public ConversationsController(IConversationService conversationService, IHubContext<ChatHub> hubContext)
    {
        _conversationService = conversationService;        
        _hubContext = hubContext;
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
        MessageDto message = new MessageDto
        {
            MessageId = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = request.SenderId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
        await _hubContext.Clients.Group(conversationId.ToString()).SendAsync("SentMessage", message);
        return Ok(message);
    }
}
