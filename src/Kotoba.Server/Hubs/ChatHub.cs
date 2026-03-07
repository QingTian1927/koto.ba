using Kotoba.Shared.DTOs;
using Kotoba.Domain.Entities;
using Kotoba.Domain.Enums;
using Kotoba.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Kotoba.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;

        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task JoinConversation(Guid conversationId)
        {
            var userId = Context.UserIdentifier;

            var isParticipant = await _db.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                 throw new HubException("You are not a participant of this conversation.");                
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task LeaveConversation(Guid conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task<ConversationDto> CreateDirectConversation(string targetUserId)
        {
            var currentUserId = Context.UserIdentifier!;
            if (currentUserId == targetUserId)
            {
                throw new HubException("You cannot create a conversation with yourself.");
            }

            var existing = await _db.Conversations
                .Include(c => c.Participants)
                .Where(c => c.Type == ConversationType.Direct
                && c.Participants.Any(p => p.UserId == currentUserId && p.IsActive)
                && c.Participants.Any(p => p.UserId == targetUserId && p.IsActive))
                .FirstOrDefaultAsync();

            if (existing != null) return await MapConversationDto(existing);

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Direct,
                CreatedAt = DateTime.UtcNow
            };

            _db.Conversations.Add(conversation);

            _db.ConversationParticipants.AddRange(
                new ConversationParticipant
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    UserId = currentUserId
                },
                new ConversationParticipant
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    UserId = targetUserId
                }
            );
            await _db.SaveChangesAsync();

            var created = await _db.Conversations
                .Include(c => c.Participants).ThenInclude(p => p.User)
                .FirstAsync(c => c.Id == conversation.Id);

            return await MapConversationDto(created);
        }

        public async Task<List<ConversationDto>> GetConversations()
        {
            var userId = Context.UserIdentifier!;

            var conversations = await _db.Conversations
                .Include(c => c.Participants).ThenInclude(p => p.User)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                    .ThenInclude(m => m.Sender)
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsActive))
                .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
                .ToListAsync();

            var result = new List<ConversationDto>();
            foreach (var c in conversations)
                result.Add(await MapConversationDto(c));

            return result;
        }

        public async Task SendMessage(SendMessageRequest request)
        {
            var userId = Context.UserIdentifier!;

            // Validate participant
            var isParticipant = await _db.ConversationParticipants
                .AnyAsync(p => p.ConversationId == request.ConversationId
                            && p.UserId == userId
                            && p.IsActive);

            if (!isParticipant)
                throw new HubException("Access denied.");

            // Save to DB
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = request.ConversationId,
                SenderId = userId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);

            // Update conversation timestamp
            var conversation = await _db.Conversations.FindAsync(request.ConversationId);
            if (conversation != null)
                conversation.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Load sender info
            var sender = await _db.Users.FindAsync(userId);

            var dto = new MessageDto { 
                MessageId = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                Status = MessageStatus.Sent
            };

            // Broadcast to all in the conversation group (including sender)
            // Client sẽ dùng TempId để replace optimistic message
            await Clients.Group(request.ConversationId.ToString())
                .SendAsync("MessageConfirmed", dto, request.TempId);
        }
        
        private async Task<ConversationDto> MapConversationDto(Conversation c)
        {
            var lastMessage = c.Messages
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            MessageDto? lastMsgDto = null;
            if (lastMessage != null)
            {
                lastMsgDto = new MessageDto {
                    MessageId = lastMessage.Id,
                    ConversationId = lastMessage.ConversationId,
                    SenderId = lastMessage.SenderId,
                    Content = lastMessage.Content,
                    CreatedAt = lastMessage.CreatedAt,
                    Status = MessageStatus.Sent
                };
            }

            return new ConversationDto {
                ConversationId = c.Id,
                Type = c.Type,
                GroupName = c.GroupName,
                Participants = c.Participants.Select(p => new UserProfile {
                    UserId = p.UserId,
                    DisplayName = p.User.DisplayName,
                    AvatarUrl = p.User.AvatarUrl,
                    IsOnline = p.User.IsOnline,
                    LastSeenAt = p.User.LastSeenAt
                }).ToList(),
                LastMessage = lastMsgDto,
                CreatedAt =c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }

    }
}
