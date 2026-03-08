using Kotoba.Domain.Enums;
using Kotoba.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Kotoba.Client.Services
{
    public class ChatService : IAsyncDisposable
    {
        private HubConnection? _hub;
        private readonly NavigationManager _nav;
        public event Action? OnChange;
        public List<ConversationDto> Conversations { get; private set; } = new();
        public Dictionary<Guid, List<MessageDto>> Messages { get; private set; } = new();
        public ChatService(NavigationManager nav)  // ← inject
        {
            _nav = nav;
        }

        public async Task StartAsync(string accessToken = "")
        {
            if (_hub?.State == HubConnectionState.Connected) return;
            _hub = new HubConnectionBuilder()
            .WithUrl(_nav.ToAbsoluteUri("/chathub"))  // ← fix URL
            .WithAutomaticReconnect()
            .Build();

            // Nhận message đã được server confirm (Sent)
            _hub.On<MessageDto, string>("MessageConfirmed", (msg, tempId) =>
            {
                if (!Messages.ContainsKey(msg.ConversationId))
                    Messages[msg.ConversationId] = new();

                var list = Messages[msg.ConversationId];

                // Replace optimistic (Pending) message bằng confirmed message
                var pendingIndex = list.FindIndex(m => m.MessageId.ToString() == tempId);
                if (pendingIndex >= 0)
                    list[pendingIndex] = msg;
                else
                    list.Add(msg); // tin nhắn từ người khác

                // Update last message trong conversation list
                var conv = Conversations.FirstOrDefault(c => c.ConversationId == msg.ConversationId);
                if (conv != null)
                {
                    var idx = Conversations.IndexOf(conv);
                    // Create a new ConversationDto copying all properties, but updating LastMessage
                    Conversations[idx] = new ConversationDto
                    {
                        ConversationId = conv.ConversationId,
                        Type = conv.Type,
                        GroupName = conv.GroupName,
                        Participants = conv.Participants,
                        LastMessage = msg,
                        CreatedAt = conv.CreatedAt,
                        UpdatedAt = conv.UpdatedAt
                    };
                }

                OnChange?.Invoke();
            });

            await _hub.StartAsync();
        }

        // ── Conversations ──────────────────────────────────────────────────
        public async Task LoadConversationsAsync()
        {
            var list = await _hub!.InvokeAsync<List<ConversationDto>>("GetConversations");
            Conversations = list;
            OnChange?.Invoke();
        }

        public async Task<ConversationDto> CreateDirectConversationAsync(string targetUserId)
        {
            var conv = await _hub!.InvokeAsync<ConversationDto>(
                "CreateDirectConversation", targetUserId);

            if (!Conversations.Any(c => c.ConversationId == conv.ConversationId))
                Conversations.Insert(0, conv);

            OnChange?.Invoke();
            return conv;
        }

        // ── Messages ───────────────────────────────────────────────────────
        public async Task JoinConversationAsync(Guid conversationId)
        {
            await _hub!.InvokeAsync("JoinConversation", conversationId);

            if (!Messages.ContainsKey(conversationId))
                Messages[conversationId] = new();
        }

        public async Task SendMessageAsync(Guid conversationId, string content, string currentUserId)
        {
            var tempId = Guid.NewGuid().ToString();

            // ── Optimistic UI: thêm message trạng thái Pending ngay lập tức ──
            var pendingMsg = new MessageDto {
                MessageId = Guid.Parse(tempId),  // dùng tempId làm Id tạm
                ConversationId = conversationId,
                SenderId = currentUserId,
                Content = content,
                CreatedAt = DateTime.Now,
                Status = MessageStatus.Pending
            };

            if (!Messages.ContainsKey(conversationId))
                Messages[conversationId] = new();

            Messages[conversationId].Add(pendingMsg);
            OnChange?.Invoke();  // render ngay

            // ── Gửi lên server ──
            await _hub!.InvokeAsync("SendMessage", new SendMessageRequest {
                ConversationId = conversationId, SenderId = currentUserId, Content = content, TempId = tempId});
        }

        public async ValueTask DisposeAsync()
        {
            if (_hub != null)
                await _hub.DisposeAsync();
        }
    }
}
