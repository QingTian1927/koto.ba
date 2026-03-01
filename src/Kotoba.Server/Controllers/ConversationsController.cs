using Kotoba.Core.Interfaces;
using Kotoba.Domain.DTOs;
using Kotoba.Server.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Kotoba.Server.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationsController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext;

    public ConversationsController(
        IConversationService conversationService,
        IMessageService messageService,
        IHubContext<ChatHub> hubContext)
    {
        _conversationService = conversationService;
        _messageService = messageService;
        _hubContext = hubContext;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<List<ConversationDto>>> GetUserConversations(string userId)
    {
        var conversations = await _conversationService.GetUserConversationsAsync(userId);
        return Ok(conversations);
    }

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        Guid conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var messages = await _messageService.GetMessagesAsync(
            conversationId,
            new PagingRequest { Page = page, PageSize = pageSize });
        return Ok(messages);
    }

    [HttpPost("{conversationId:guid}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(
        Guid conversationId,
        [FromBody] SendMessageRequest request)
    {
        request.ConversationId = conversationId;
        var message = await _messageService.SendMessageAsync(request);
        if (message == null)
            return BadRequest("Failed to send message.");

        await _hubContext.Clients
            .Group(conversationId.ToString())
            .SendAsync("SentMessage", message);
        return Ok(message);
    }
}
