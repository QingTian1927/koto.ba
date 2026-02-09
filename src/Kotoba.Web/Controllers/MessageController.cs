using Kotoba.Application.DTOs;
using Kotoba.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kotoba.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        //[HttpPost("send")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        //{

        //    request.SenderId = "test-user-001";

        //    var result = await _messageService.SendMessageAsync(request);

        //    if (result == null)
        //        return BadRequest(new { message = "Failed to send message" });

        //    return Ok(result);
        //}

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            request.SenderId = userId;

            var result = await _messageService.SendMessageAsync(request);

            if (result == null)
                return BadRequest(new { message = "Failed to send message" });

            return Ok(result);
        }

        [HttpGet("{conversationId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessages(
            Guid conversationId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)  
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Invalid paging" });

            var paging = new PagingRequest { Page = page, PageSize = pageSize };
            var messages = await _messageService.GetMessagesAsync(conversationId, paging);

            return Ok(new
            {
                conversationId,
                page,
                pageSize,
                messages,
                total = messages.Count
            });
        }


    }
}
