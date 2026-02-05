# koto.ba - Team Onboarding Guide

## Foundation Complete ✅

**Dũng** has completed the foundational work for the project:
- ✅ All service interfaces defined
- ✅ Core domain entities created
- ✅ Shared DTOs and contracts established
- ✅ Enums and value objects defined
- ✅ Database configuration ready

**Status:** Ready for parallel implementation by all team members.

---

## Quick Start for All Team Members

### 1. Clone and Setup

```powershell
# Clone the repository
git clone <repository-url>
cd koto.ba

# Pull latest from develop
git checkout develop
git pull origin develop

# Restore packages
dotnet restore

# Apply database migrations
dotnet ef database update --project src/Kotoba.Infrastructure/Kotoba.Infrastructure.csproj --startup-project src/Kotoba.Web/Kotoba.Web.csproj

# Configure your database connection in appsettings.json or User Secrets
# See README.md for details

# Verify everything builds
dotnet build
```

### 2. Create Your Feature Branch

```powershell
# Create your subsystem branch
git checkout -b feature/your-subsystem-name

# Examples:
# git checkout -b feature/conversation-management    (Nga)
# git checkout -b feature/message-reactions          (Vinh)
# git checkout -b feature/realtime-ai                (Hoàn)
```

---

## Architecture Overview

### Project Structure

```
koto.ba/
├── src/
│   ├── Kotoba.Domain/              ✅ COMPLETE - Entities & Enums (DO NOT MODIFY)
│   ├── Kotoba.Application/         ✅ COMPLETE - Interfaces & DTOs (DO NOT MODIFY)
│   ├── Kotoba.Infrastructure/      🔧 YOUR WORK - Implement services here
│   ├── Kotoba.Infrastructure.AI/   🔧 YOUR WORK - AI implementation (Hoàn only)
│   └── Kotoba.Web/                 🔧 YOUR WORK - Blazor UI components
```

### Dependency Rules (CRITICAL)

**✅ ALLOWED:**
- Infrastructure → Application (to implement interfaces)
- Infrastructure → Domain (to use entities)
- Web → Everything (composition root)

**❌ FORBIDDEN:**
- Domain → Anything
- Application → Infrastructure
- Infrastructure ↔ Infrastructure.AI (subsystems must not reference each other)

---

## Nga - Conversation Management

### Your Responsibility

Implement conversation creation and management (1-1 and group chats).

### Interface to Implement

**File:** `src/Kotoba.Application/Interfaces/IConversationService.cs`

```csharp
public interface IConversationService
{
    Task<ConversationDto?> CreateDirectConversationAsync(string userAId, string userBId);
    Task<ConversationDto?> CreateGroupConversationAsync(CreateGroupRequest request);
    Task<List<ConversationDto>> GetUserConversationsAsync(string userId);
    Task<ConversationDto?> GetConversationDetailAsync(Guid conversationId);
}
```

### Entities You'll Work With

- `Conversation` - Main conversation entity
- `ConversationParticipant` - Junction table for users in conversations
- `User` - For participant information

### Step-by-Step Implementation

#### Step 1: Create Your Service Folder

```powershell
New-Item -Path "src/Kotoba.Infrastructure/Services/Conversations" -ItemType Directory -Force
```

#### Step 2: Create the Service Implementation

```powershell
notepad src/Kotoba.Infrastructure/Services/Conversations/ConversationService.cs
```

**Basic structure to start:**

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Kotoba.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Conversations;

public class ConversationService : IConversationService
{
    private readonly ApplicationDbContext _context;

    public ConversationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConversationDto?> CreateDirectConversationAsync(string userAId, string userBId)
    {
        // TODO: Check if direct conversation already exists between these two users
        // TODO: If not, create new Conversation with Type = Direct
        // TODO: Add both users as ConversationParticipants
        // TODO: Save to database
        // TODO: Return ConversationDto

        throw new NotImplementedException();
    }

    public async Task<ConversationDto?> CreateGroupConversationAsync(CreateGroupRequest request)
    {
        // TODO: Create new Conversation with Type = Group
        // TODO: Set GroupName from request
        // TODO: Add all participants from request.ParticipantIds
        // TODO: Save to database
        // TODO: Return ConversationDto

        throw new NotImplementedException();
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(string userId)
    {
        // TODO: Query conversations where user is a participant
        // TODO: Include participant information
        // TODO: Order by UpdatedAt descending
        // TODO: Map to ConversationDto list

        throw new NotImplementedException();
    }

    public async Task<ConversationDto?> GetConversationDetailAsync(Guid conversationId)
    {
        // TODO: Find conversation by Id
        // TODO: Include all participants
        // TODO: Map to ConversationDto

        throw new NotImplementedException();
    }
}
```

#### Step 3: Register Your Service

Create an extension method for dependency injection:

```powershell
notepad src/Kotoba.Infrastructure/Services/Conversations/ConversationServiceExtensions.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Kotoba.Infrastructure.Services.Conversations;

public static class ConversationServiceExtensions
{
    public static IServiceCollection AddConversationServices(this IServiceCollection services)
    {
        services.AddScoped<IConversationService, ConversationService>();
        return services;
    }
}
```

#### Step 4: Register in Program.cs

Add this line in `src/Kotoba.Web/Program.cs` after the Identity configuration:

```csharp
// Nga's Conversation Services
builder.Services.AddConversationServices();
```

#### Step 5: Add DbSets to ApplicationDbContext

Open `src/Kotoba.Infrastructure/Data/ApplicationDbContext.cs` and add:

```csharp
public DbSet<Conversation> Conversations { get; set; }
public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
```

#### Step 6: Create Entity Configurations (Optional but Recommended)

```powershell
New-Item -Path "src/Kotoba.Infrastructure/Data/Configurations" -ItemType Directory -Force
notepad src/Kotoba.Infrastructure/Data/Configurations/ConversationConfiguration.cs
```

```csharp
using Kotoba.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kotoba.Infrastructure.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.GroupName).HasMaxLength(100);
        builder.HasIndex(c => c.CreatedAt);

        // Configure relationships
        builder.HasMany(c => c.Participants)
            .WithOne(cp => cp.Conversation)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

```powershell
notepad src/Kotoba.Infrastructure/Data/Configurations/ConversationParticipantConfiguration.cs
```

```csharp
using Kotoba.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kotoba.Infrastructure.Data.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.HasKey(cp => cp.Id);

        // Composite index for finding conversations by user
        builder.HasIndex(cp => new { cp.UserId, cp.ConversationId });

        // Configure relationships
        builder.HasOne(cp => cp.User)
            .WithMany(u => u.ConversationParticipants)
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

Register configurations in ApplicationDbContext:

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder.ApplyConfiguration(new ConversationConfiguration());
    builder.ApplyConfiguration(new ConversationParticipantConfiguration());
}
```

#### Step 7: Create Migration

```powershell
dotnet ef migrations add AddConversationEntities --project src/Kotoba.Infrastructure/Kotoba.Infrastructure.csproj --startup-project src/Kotoba.Web/Kotoba.Web.csproj
```

#### Step 8: Apply Migration

```powershell
dotnet ef database update --project src/Kotoba.Infrastructure/Kotoba.Infrastructure.csproj --startup-project src/Kotoba.Web/Kotoba.Web.csproj
```

#### Step 9: Test Your Implementation

Create a simple test or use the application to verify:
- Creating direct conversations
- Creating group conversations
- Retrieving user's conversations

### Tips for Nga

1. **Check for existing direct conversations** before creating a new one (avoid duplicates)
2. **Use transactions** when creating conversations with multiple participants
3. **Eager load participants** when querying to avoid N+1 queries
4. **Consider soft deletes** - set `IsActive = false` instead of deleting

### What to Commit

```powershell
git add src/Kotoba.Infrastructure/Services/Conversations/
git add src/Kotoba.Infrastructure/Data/Configurations/Conversation*.cs
git add src/Kotoba.Infrastructure/Data/ApplicationDbContext.cs
git add src/Kotoba.Infrastructure/Data/Migrations/
git commit -m "feat: Implement ConversationService

- Add CreateDirectConversationAsync
- Add CreateGroupConversationAsync
- Add GetUserConversationsAsync
- Add GetConversationDetailAsync
- Add entity configurations
- Add database migration"

git push origin feature/conversation-management
```

---

## Vinh - Message Persistence & Reactions & Attachments

### Your Responsibility

You have two main areas:
1. **Message Persistence & History** - Store and retrieve messages
2. **Reactions & Attachments** - Handle reactions and file uploads

### Interfaces to Implement

**IMessageService:**
```csharp
public interface IMessageService
{
    Task<MessageDto?> SendMessageAsync(SendMessageRequest request);
    Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, PagingRequest paging);
}
```

**IReactionService:**
```csharp
public interface IReactionService
{
    Task<ReactionDto?> AddOrUpdateReactionAsync(string userId, Guid messageId, ReactionType reactionType);
    Task<bool> RemoveReactionAsync(string userId, Guid messageId);
    Task<List<ReactionDto>> GetReactionsAsync(Guid messageId);
}
```

**IAttachmentService:**
```csharp
public interface IAttachmentService
{
    Task<AttachmentDto?> UploadAttachmentAsync(UploadAttachmentRequest request);
    Task<List<AttachmentDto>> GetAttachmentsAsync(Guid messageId);
}
```

### Entities You'll Work With

- `Message` - Main message entity
- `Reaction` - User reactions to messages
- `Attachment` - File attachments

### Step-by-Step Implementation

#### Part 1: Message Service

##### Step 1: Create Service Folders

```powershell
New-Item -Path "src/Kotoba.Infrastructure/Services/Messages" -ItemType Directory -Force
New-Item -Path "src/Kotoba.Infrastructure/Services/Reactions" -ItemType Directory -Force
New-Item -Path "src/Kotoba.Infrastructure/Services/Attachments" -ItemType Directory -Force
```

##### Step 2: Implement MessageService

```powershell
notepad src/Kotoba.Infrastructure/Services/Messages/MessageService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Kotoba.Infrastructure.Data;
using Kotoba.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Messages;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _context;

    public MessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MessageDto?> SendMessageAsync(SendMessageRequest request)
    {
        // TODO: Validate conversation exists
        // TODO: Validate sender is participant in conversation
        // TODO: Create new Message entity
        // TODO: Save to database
        // TODO: Update conversation UpdatedAt timestamp
        // TODO: Return MessageDto

        throw new NotImplementedException();
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, PagingRequest paging)
    {
        // TODO: Query messages for conversation
        // TODO: Order by CreatedAt descending
        // TODO: Apply pagination (Skip/Take)
        // TODO: Map to MessageDto list
        // TODO: Return in chronological order (reverse after pagination)

        throw new NotImplementedException();
    }
}
```

#### Part 2: Reaction Service

```powershell
notepad src/Kotoba.Infrastructure/Services/Reactions/ReactionService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Kotoba.Domain.Enums;
using Kotoba.Infrastructure.Data;
using Kotoba.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Reactions;

public class ReactionService : IReactionService
{
    private readonly ApplicationDbContext _context;

    public ReactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReactionDto?> AddOrUpdateReactionAsync(string userId, Guid messageId, ReactionType reactionType)
    {
        // TODO: Check if user already has reaction on this message
        // TODO: If exists, update the reaction type
        // TODO: If not, create new Reaction entity
        // TODO: Save to database
        // TODO: Return ReactionDto

        // IMPORTANT: Each user can only have ONE reaction per message

        throw new NotImplementedException();
    }

    public async Task<bool> RemoveReactionAsync(string userId, Guid messageId)
    {
        // TODO: Find existing reaction
        // TODO: Remove from database
        // TODO: Return success status

        throw new NotImplementedException();
    }

    public async Task<List<ReactionDto>> GetReactionsAsync(Guid messageId)
    {
        // TODO: Query all reactions for message
        // TODO: Include user information
        // TODO: Map to ReactionDto list

        throw new NotImplementedException();
    }
}
```

#### Part 3: Attachment Service

```powershell
notepad src/Kotoba.Infrastructure/Services/Attachments/AttachmentService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Kotoba.Infrastructure.Data;
using Kotoba.Domain.Entities;
using Kotoba.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Attachments;

public class AttachmentService : IAttachmentService
{
    private readonly ApplicationDbContext _context;
    private readonly string _uploadPath;

    public AttachmentService(ApplicationDbContext context)
    {
        _context = context;
        // TODO: Get upload path from configuration
        _uploadPath = Path.Combine("wwwroot", "uploads");

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<AttachmentDto?> UploadAttachmentAsync(UploadAttachmentRequest request)
    {
        // TODO: Validate file type (images: PNG, JPG; documents: PDF)
        // TODO: Generate unique filename (use Guid to avoid conflicts)
        // TODO: Determine FileType enum based on extension
        // TODO: Save file to disk
        // TODO: Create Attachment entity with file info
        // TODO: Save to database
        // TODO: Return AttachmentDto with file URL

        // Example file path: /uploads/{guid}_{originalfilename}

        throw new NotImplementedException();
    }

    public async Task<List<AttachmentDto>> GetAttachmentsAsync(Guid messageId)
    {
        // TODO: Query attachments for message
        // TODO: Map to AttachmentDto list

        throw new NotImplementedException();
    }
}
```

##### Step 3: Create Extension Methods

```powershell
notepad src/Kotoba.Infrastructure/Services/Messages/MessageServiceExtensions.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Kotoba.Infrastructure.Services.Messages;

public static class MessageServiceExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
        return services;
    }
}
```

Do the same for ReactionService and AttachmentService.

##### Step 4: Register Services in Program.cs

```csharp
// Vinh's Message & Reaction Services
builder.Services.AddMessageServices();
builder.Services.AddReactionServices();
builder.Services.AddAttachmentServices();
```

##### Step 5: Add DbSets to ApplicationDbContext

```csharp
public DbSet<Message> Messages { get; set; }
public DbSet<Reaction> Reactions { get; set; }
public DbSet<Attachment> Attachments { get; set; }
```

##### Step 6: Create Entity Configurations

Create configurations for Message, Reaction, and Attachment entities following the same pattern as Nga's conversation configurations.

**Key points:**
- Message: Index on ConversationId and CreatedAt
- Reaction: Unique index on (UserId, MessageId) to enforce one reaction per user per message
- Attachment: Index on MessageId

##### Step 7: Create and Apply Migration

```powershell
dotnet ef migrations add AddMessageReactionAttachmentEntities --project src/Kotoba.Infrastructure/Kotoba.Infrastructure.csproj --startup-project src/Kotoba.Web/Kotoba.Web.csproj

dotnet ef database update --project src/Kotoba.Infrastructure/Kotoba.Infrastructure.csproj --startup-project src/Kotoba.Web/Kotoba.Web.csproj
```

### Tips for Vinh

1. **Message pagination:** Use descending order, then reverse the list for display
2. **File validation:** Check file extensions and size limits
3. **Unique filenames:** Use `Guid.NewGuid()` to prevent filename conflicts
4. **Reaction constraint:** Use a unique index on (UserId, MessageId) to enforce one reaction per user
5. **Transaction safety:** Use transactions when updating multiple related entities

### File Upload Best Practices

```csharp
// Example file upload logic
var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
var allowedDocExtensions = new[] { ".pdf" };
var maxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

// Validate file type
if (!allowedImageExtensions.Contains(extension) && !allowedDocExtensions.Contains(extension))
{
    throw new InvalidOperationException("File type not allowed");
}

// Validate file size
if (request.File.Length > maxFileSizeBytes)
{
    throw new InvalidOperationException("File size exceeds limit");
}

// Generate unique filename
var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";
var filePath = Path.Combine(_uploadPath, uniqueFileName);

// Save file
using (var stream = new FileStream(filePath, FileMode.Create))
{
    await request.File.CopyToAsync(stream);
}
```

### What to Commit

```powershell
git add src/Kotoba.Infrastructure/Services/Messages/
git add src/Kotoba.Infrastructure/Services/Reactions/
git add src/Kotoba.Infrastructure/Services/Attachments/
git add src/Kotoba.Infrastructure/Data/Configurations/
git add src/Kotoba.Infrastructure/Data/ApplicationDbContext.cs
git add src/Kotoba.Infrastructure/Data/Migrations/
git commit -m "feat: Implement MessageService, ReactionService, and AttachmentService

- Add SendMessageAsync and GetMessagesAsync
- Add reaction CRUD operations (one per user per message)
- Add file upload for images and PDFs
- Add entity configurations
- Add database migration"

git push origin feature/message-reactions
```

---

## Hoàn - Realtime Interaction & AI & Social Features

### Your Responsibility

You have the most diverse set of features:
1. **SignalR ChatHub** - Real-time message broadcasting and typing indicators
2. **AI Reply Suggestions** - Generate AI-powered reply suggestions
3. **Social Features** - Stories and Current Thoughts

### Interfaces to Implement

**IRealtimeChatService:**
```csharp
public interface IRealtimeChatService
{
    Task BroadcastMessageAsync(MessageDto message);
    Task BroadcastTypingAsync(TypingStatusDto typingStatus);
}
```

**ITypingService:**
```csharp
public interface ITypingService
{
    Task SetTypingAsync(string userId, Guid conversationId, bool isTyping);
}
```

**IAIReplyService:**
```csharp
public interface IAIReplyService
{
    Task<List<string>> GenerateSuggestionsAsync(AIReplyRequest request);
}
```

**IStoryService:**
```csharp
public interface IStoryService
{
    Task<StoryDto?> CreateStoryAsync(CreateStoryRequest request);
    Task<List<StoryDto>> GetActiveStoriesAsync();
}
```

**ICurrentThoughtService:**
```csharp
public interface ICurrentThoughtService
{
    Task<bool> SetThoughtAsync(string userId, string content);
    Task<string?> GetThoughtAsync(string userId);
}
```

### Entities You'll Work With

- `Story` - User stories (24-hour expiration)
- `CurrentThought` - One thought per user (auto-expires)

### Step-by-Step Implementation

#### Part 1: SignalR Hub

##### Step 1: Create SignalR Hub

```powershell
New-Item -Path "src/Kotoba.Infrastructure/Hubs" -ItemType Directory -Force
notepad src/Kotoba.Infrastructure/Hubs/ChatHub.cs
```

```csharp
using Microsoft.AspNetCore.SignalR;
using Kotoba.Application.DTOs;

namespace Kotoba.Infrastructure.Hubs;

public class ChatHub : Hub
{
    // Client methods (called by clients)

    public async Task JoinConversation(string conversationId)
    {
        // Add connection to conversation group
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        // Remove connection from conversation group
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendTypingStatus(TypingStatusDto status)
    {
        // Broadcast typing status to conversation group (except sender)
        await Clients.OthersInGroup(status.ConversationId.ToString())
            .SendAsync("TypingStatusChanged", status);
    }

    // Server methods (called by IRealtimeChatService)
    // These will be called from the service layer, not directly by clients
}
```

##### Step 2: Implement RealtimeChatService

```powershell
New-Item -Path "src/Kotoba.Infrastructure/Services/Realtime" -ItemType Directory -Force
notepad src/Kotoba.Infrastructure/Services/Realtime/RealtimeChatService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Kotoba.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kotoba.Infrastructure.Services.Realtime;

public class RealtimeChatService : IRealtimeChatService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public RealtimeChatService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastMessageAsync(MessageDto message)
    {
        // Broadcast new message to all clients in the conversation
        await _hubContext.Clients
            .Group(message.ConversationId.ToString())
            .SendAsync("MessageReceived", message);
    }

    public async Task BroadcastTypingAsync(TypingStatusDto typingStatus)
    {
        // Broadcast typing status to conversation
        await _hubContext.Clients
            .Group(typingStatus.ConversationId.ToString())
            .SendAsync("TypingStatusChanged", typingStatus);
    }
}
```

##### Step 3: Implement TypingService

```powershell
notepad src/Kotoba.Infrastructure/Services/Realtime/TypingService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace Kotoba.Infrastructure.Services.Realtime;

public class TypingService : ITypingService
{
    private readonly IMemoryCache _cache;
    private readonly IRealtimeChatService _realtimeService;

    public TypingService(IMemoryCache cache, IRealtimeChatService realtimeService)
    {
        _cache = cache;
        _realtimeService = realtimeService;
    }

    public async Task SetTypingAsync(string userId, Guid conversationId, bool isTyping)
    {
        var cacheKey = $"typing_{conversationId}_{userId}";

        if (isTyping)
        {
            // Cache typing status for 5 seconds
            _cache.Set(cacheKey, true, TimeSpan.FromSeconds(5));
        }
        else
        {
            // Remove from cache when user stops typing
            _cache.Remove(cacheKey);
        }

        // Broadcast typing status
        var status = new TypingStatusDto
        {
            UserId = userId,
            ConversationId = conversationId,
            IsTyping = isTyping
        };

        await _realtimeService.BroadcastTypingAsync(status);
    }
}
```

##### Step 4: Register SignalR in Program.cs

In `src/Kotoba.Web/Program.cs`:

```csharp
// SignalR (already added, but ensure it's there)
builder.Services.AddSignalR();

// Hoàn's Realtime Services
builder.Services.AddScoped<IRealtimeChatService, RealtimeChatService>();
builder.Services.AddScoped<ITypingService, TypingService>();
```

And after `app.UseAuthorization();`:

```csharp
// Map SignalR Hub
app.MapHub<ChatHub>("/chathub");
```

#### Part 2: AI Reply Service

##### Step 1: Create AI Service in Infrastructure.AI Project

```powershell
New-Item -Path "src/Kotoba.Infrastructure.AI/Services" -ItemType Directory -Force
notepad src/Kotoba.Infrastructure.AI/Services/AIReplyService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Kotoba.Domain.Enums;
using System.Text.Json;

namespace Kotoba.Infrastructure.AI.Services;

public class AIReplyService : IAIReplyService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiEndpoint;

    public AIReplyService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("AIClient");
        _apiKey = configuration["AI:ApiKey"] ?? throw new InvalidOperationException("AI API Key not configured");
        _apiEndpoint = configuration["AI:Endpoint"] ?? throw new InvalidOperationException("AI Endpoint not configured");
    }

    public async Task<List<string>> GenerateSuggestionsAsync(AIReplyRequest request)
    {
        // TODO: Build prompt based on tone
        var prompt = BuildPrompt(request.OriginalMessage, request.Tone);

        // TODO: Call external AI API (e.g., OpenAI, Anthropic, etc.)
        // TODO: Parse response and extract suggestions
        // TODO: Return list of 3-5 suggested replies

        // PLACEHOLDER: Return mock suggestions for now
        return new List<string>
        {
            GetMockReply(request.Tone, 1),
            GetMockReply(request.Tone, 2),
            GetMockReply(request.Tone, 3)
        };
    }

    private string BuildPrompt(string originalMessage, AITone tone)
    {
        var toneDescription = tone switch
        {
            AITone.Polite => "polite and formal",
            AITone.Friendly => "friendly and casual",
            AITone.Confident => "confident and assertive",
            _ => "neutral"
        };

        return $"Generate 3 {toneDescription} replies to this message: '{originalMessage}'";
    }

    private string GetMockReply(AITone tone, int index)
    {
        // TODO: Replace with actual AI API call
        return tone switch
        {
            AITone.Polite => $"Thank you for your message. Reply {index}",
            AITone.Friendly => $"Hey! Reply {index}",
            AITone.Confident => $"Sure thing! Reply {index}",
            _ => $"Reply {index}"
        };
    }
}
```

##### Step 2: Configure AI Settings

Add to `appsettings.json`:

```json
{
  "AI": {
    "ApiKey": "your-api-key-here",
    "Endpoint": "https://api.example.com/v1/completions"
  }
}
```

**IMPORTANT:** Use User Secrets for API keys, never commit them!

```powershell
dotnet user-secrets set "AI:ApiKey" "your-actual-api-key" --project src/Kotoba.Web/Kotoba.Web.csproj
```

##### Step 3: Register AI Service

```powershell
notepad src/Kotoba.Infrastructure.AI/Services/AIServiceExtensions.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Kotoba.Infrastructure.AI.Services;

public static class AIServiceExtensions
{
    public static IServiceCollection AddAIServices(this IServiceCollection services)
    {
        services.AddHttpClient("AIClient");
        services.AddScoped<IAIReplyService, AIReplyService>();
        return services;
    }
}
```

In `Program.cs`:

```csharp
// Hoàn's AI Services
builder.Services.AddAIServices();
```

#### Part 3: Story and Current Thought Services

##### Step 1: Create Social Services

```powershell
New-Item -Path "src/Kotoba.Infrastructure/Services/Social" -ItemType Directory -Force
notepad src/Kotoba.Infrastructure/Services/Social/StoryService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Application.DTOs;
using Kotoba.Infrastructure.Data;
using Kotoba.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Social;

public class StoryService : IStoryService
{
    private readonly ApplicationDbContext _context;

    public StoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StoryDto?> CreateStoryAsync(CreateStoryRequest request)
    {
        // TODO: Check if user has existing active story (optional: allow multiple)
        // TODO: Create new Story entity
        // TODO: Set ExpiresAt to 24 hours from now
        // TODO: Save to database
        // TODO: Return StoryDto

        throw new NotImplementedException();
    }

    public async Task<List<StoryDto>> GetActiveStoriesAsync()
    {
        // TODO: Query stories where ExpiresAt > DateTime.UtcNow
        // TODO: Order by CreatedAt descending
        // TODO: Include user information
        // TODO: Map to StoryDto list

        throw new NotImplementedException();
    }
}
```

```powershell
notepad src/Kotoba.Infrastructure/Services/Social/CurrentThoughtService.cs
```

```csharp
using Kotoba.Application.Interfaces;
using Kotoba.Infrastructure.Data;
using Kotoba.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kotoba.Infrastructure.Services.Social;

public class CurrentThoughtService : ICurrentThoughtService
{
    private readonly ApplicationDbContext _context;

    public CurrentThoughtService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SetThoughtAsync(string userId, string content)
    {
        // TODO: Find existing CurrentThought for user
        // TODO: If exists, update content and reset ExpiresAt
        // TODO: If not, create new CurrentThought
        // TODO: Set ExpiresAt (e.g., 1 hour from now)
        // TODO: Save to database

        throw new NotImplementedException();
    }

    public async Task<string?> GetThoughtAsync(string userId)
    {
        // TODO: Find CurrentThought for user
        // TODO: Check if expired
        // TODO: If expired, delete and return null
        // TODO: If active, return content

        throw new NotImplementedException();
    }
}
```

##### Step 2: Add DbSets

```csharp
public DbSet<Story> Stories { get; set; }
public DbSet<CurrentThought> CurrentThoughts { get; set; }
```

##### Step 3: Create Entity Configurations

Create configurations for Story and CurrentThought.

**Key points:**
- Story: Index on UserId and ExpiresAt
- CurrentThought: Unique index on UserId (one thought per user)

##### Step 4: Register Services

```csharp
// Hoàn's Social Services
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<ICurrentThoughtService, CurrentThoughtService>();
```

##### Step 5: Create and Apply Migration

```powershell
dotnet ef migrations add AddStoryAndCurrentThoughtEntities --project src/Kotoba.Infrastructure/Kotoba.Infrastructure.csproj --startup-project src/Kotoba.Web/Kotoba.Web.csproj

dotnet ef database update --project src/Kotoba.Infrastructure/Kotoba.Infrastructure.csproj --startup-project src/Kotoba.Web/Kotoba.Web.csproj
```

#### Part 4: Background Service for Auto-Expiration (Optional but Recommended)

Create a background service to clean up expired stories and thoughts:

```powershell
New-Item -Path "src/Kotoba.Infrastructure/BackgroundServices" -ItemType Directory -Force
notepad src/Kotoba.Infrastructure/BackgroundServices/ExpirationCleanupService.cs
```

```csharp
using Kotoba.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kotoba.Infrastructure.BackgroundServices;

public class ExpirationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExpirationCleanupService> _logger;

    public ExpirationCleanupService(IServiceProvider serviceProvider, ILogger<ExpirationCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredItemsAsync();
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken); // Run every 15 minutes
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during expiration cleanup");
            }
        }
    }

    private async Task CleanupExpiredItemsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;

        // Remove expired stories
        var expiredStories = await context.Stories
            .Where(s => s.ExpiresAt <= now)
            .ToListAsync();

        if (expiredStories.Any())
        {
            context.Stories.RemoveRange(expiredStories);
            _logger.LogInformation($"Removed {expiredStories.Count} expired stories");
        }

        // Remove expired current thoughts
        var expiredThoughts = await context.CurrentThoughts
            .Where(ct => ct.ExpiresAt <= now)
            .ToListAsync();

        if (expiredThoughts.Any())
        {
            context.CurrentThoughts.RemoveRange(expiredThoughts);
            _logger.LogInformation($"Removed {expiredThoughts.Count} expired thoughts");
        }

        await context.SaveChangesAsync();
    }
}
```

Register in `Program.cs`:

```csharp
builder.Services.AddHostedService<ExpirationCleanupService>();
```

### Tips for Hoàn

1. **SignalR Groups:** Use conversation IDs as group names for targeted broadcasting
2. **Typing indicators:** Cache them temporarily (3-5 seconds) to reduce noise
3. **AI API:** Start with mock data, then integrate real AI API later
4. **Story expiration:** Use background service for cleanup, but also check on-read
5. **Error handling:** Wrap AI API calls in try-catch, return empty list on failure

### What to Commit

```powershell
git add src/Kotoba.Infrastructure/Hubs/
git add src/Kotoba.Infrastructure/Services/Realtime/
git add src/Kotoba.Infrastructure/Services/Social/
git add src/Kotoba.Infrastructure.AI/Services/
git add src/Kotoba.Infrastructure/BackgroundServices/
git add src/Kotoba.Infrastructure/Data/Configurations/
git add src/Kotoba.Infrastructure/Data/ApplicationDbContext.cs
git add src/Kotoba.Infrastructure/Data/Migrations/
git commit -m "feat: Implement RealtimeChatService, AI, Story, and CurrentThought

- Add SignalR ChatHub for realtime communication
- Add typing indicator with memory cache
- Add AI reply suggestion service (with mock data)
- Add Story service with 24-hour expiration
- Add CurrentThought service (one per user)
- Add background cleanup service
- Add entity configurations
- Add database migration"

git push origin feature/realtime-ai
```

---

## Integration Guide: How Services Work Together

### Typical Message Flow

1. **User sends message (Vinh's code):**
   ```csharp
   var message = await _messageService.SendMessageAsync(request);
   ```

2. **Broadcast to all participants (Hoàn's code):**
   ```csharp
   await _realtimeChatService.BroadcastMessageAsync(message);
   ```

3. **Other users receive via SignalR client (Hoàn's code):**
   ```javascript
   connection.on("MessageReceived", (message) => {
       // Display message in UI
   });
   ```

### Typical AI Suggestion Flow

1. **User requests AI suggestions (Hoàn's code):**
   ```csharp
   var suggestions = await _aiReplyService.GenerateSuggestionsAsync(request);
   ```

2. **User selects a suggestion and sends (Vinh's code):**
   ```csharp
   var message = await _messageService.SendMessageAsync(new SendMessageRequest {
       Content = selectedSuggestion
   });
   ```

### Integration Example in Blazor Component

```csharp
@inject IMessageService MessageService
@inject IRealtimeChatService RealtimeService
@inject IAIReplyService AIService

@code {
    private async Task SendMessageAsync()
    {
        // 1. Save message (Vinh's service)
        var message = await MessageService.SendMessageAsync(new SendMessageRequest
        {
            ConversationId = currentConversationId,
            SenderId = currentUserId,
            Content = messageText
        });

        // 2. Broadcast realtime (Hoàn's service)
        if (message != null)
        {
            await RealtimeService.BroadcastMessageAsync(message);
        }

        messageText = string.Empty;
    }

    private async Task GetAISuggestionsAsync()
    {
        // Get AI suggestions (Hoàn's service)
        var suggestions = await AIService.GenerateSuggestionsAsync(new AIReplyRequest
        {
            UserId = currentUserId,
            OriginalMessage = lastReceivedMessage,
            Tone = AITone.Friendly
        });

        // Display suggestions to user
        aiSuggestions = suggestions;
    }
}
```

---

## Common Pitfalls & Solutions

### ❌ Problem: Circular dependencies between services

**Solution:** Services should NOT call each other directly. Use events or the controller/component layer to orchestrate.

**Example:**
```csharp
// ❌ BAD - MessageService calling RealtimeChatService directly
public class MessageService
{
    private readonly IRealtimeChatService _realtimeService; // Don't do this!
}

// ✅ GOOD - Controller orchestrating both services
public class ChatController
{
    private readonly IMessageService _messageService;
    private readonly IRealtimeChatService _realtimeService;

    public async Task<IActionResult> SendMessage(SendMessageRequest request)
    {
        var message = await _messageService.SendMessageAsync(request);
        await _realtimeService.BroadcastMessageAsync(message);
        return Ok(message);
    }
}
```

### ❌ Problem: N+1 query problem

**Solution:** Use `.Include()` for eager loading

```csharp
// ❌ BAD
var conversations = await _context.Conversations
    .Where(c => c.Participants.Any(p => p.UserId == userId))
    .ToListAsync();
// Each conversation will trigger separate query for participants

// ✅ GOOD
var conversations = await _context.Conversations
    .Include(c => c.Participants)
        .ThenInclude(p => p.User)
    .Where(c => c.Participants.Any(p => p.UserId == userId))
    .ToListAsync();
```

### ❌ Problem: Forgetting to update Conversation.UpdatedAt

**Solution:** Update timestamp when new message is sent

```csharp
var message = new Message { /* ... */ };
_context.Messages.Add(message);

// Update conversation timestamp
var conversation = await _context.Conversations.FindAsync(request.ConversationId);
if (conversation != null)
{
    conversation.UpdatedAt = DateTime.UtcNow;
}

await _context.SaveChangesAsync();
```

### ❌ Problem: Not validating user permissions

**Solution:** Always check if user is participant before allowing actions

```csharp
public async Task<MessageDto?> SendMessageAsync(SendMessageRequest request)
{
    // Validate sender is participant in conversation
    var isParticipant = await _context.ConversationParticipants
        .AnyAsync(cp => cp.ConversationId == request.ConversationId
                     && cp.UserId == request.SenderId
                     && cp.IsActive);

    if (!isParticipant)
    {
        throw new UnauthorizedAccessException("User is not a participant in this conversation");
    }

    // Proceed with sending message...
}
```

---

## Testing Your Implementation

### Manual Testing Checklist

**Nga (Conversations):**
- [ ] Create direct conversation between two users
- [ ] Create group conversation with 3+ users
- [ ] Retrieve list of conversations for a user
- [ ] Get conversation details with all participants

**Vinh (Messages & Reactions):**
- [ ] Send message to conversation
- [ ] Retrieve paginated message history
- [ ] Add reaction to message
- [ ] Update existing reaction
- [ ] Remove reaction
- [ ] Upload image file
- [ ] Upload PDF file
- [ ] Retrieve attachments for message

**Hoàn (Realtime & AI):**
- [ ] Connect to SignalR hub
- [ ] Receive message broadcast
- [ ] Send typing indicator
- [ ] Receive typing indicator
- [ ] Generate AI suggestions with different tones
- [ ] Create story (verify 24h expiration)
- [ ] Set current thought (verify one per user)
- [ ] Verify expired content is cleaned up

### Integration Testing

Create a simple test scenario:
1. User A creates conversation with User B (Nga)
2. User A sends message (Vinh)
3. User B receives message in realtime (Hoàn)
4. User B gets AI suggestions (Hoàn)
5. User B adds reaction to message (Vinh)
6. User A sees reaction update (Hoàn)

---

## Pull Request Guidelines

### Before Creating PR

```powershell
# Update from develop
git checkout develop
git pull origin develop
git checkout feature/your-subsystem
git merge develop

# Resolve any conflicts
# Build and test
dotnet build
dotnet test

# Commit any merge fixes
git add .
git commit -m "merge: Resolve conflicts with develop"
```

### PR Description Template

```markdown
## Summary
Brief description of what you implemented

## Subsystem
[Identity | Conversations | Messages | Reactions | Realtime | AI | Social]

## Changes
- [ ] Implemented IServiceName interface
- [ ] Added entity configurations
- [ ] Created database migration
- [ ] Added service registration
- [ ] Tested manually

## Testing
How did you test this?
- [ ] Unit tests (if applicable)
- [ ] Manual testing in Blazor UI
- [ ] Integration with other subsystems

## Screenshots (if UI-related)
Add screenshots if you built UI components

## Notes
Any special notes or TODOs for reviewers
```

### Review Process

1. At least **1 approval** required before merge
2. Reviewer checks:
    - Code follows conventions
    - No direct subsystem-to-subsystem calls
    - Proper error handling
    - Database migrations included
    - Service registered in Program.cs

---

## Communication & Coordination

### Daily Standup (Recommended)

**What to share:**
- What I did yesterday
- What I'm doing today
- Any blockers or questions

**Example:**
> Yesterday: Implemented CreateDirectConversationAsync and CreateGroupConversationAsync
> Today: Working on GetUserConversationsAsync with pagination
> Blockers: None, but have a question about how to handle deleted participants

### When to Coordinate

**MUST coordinate:**
- Changing any interface
- Adding new shared DTO
- Modifying ApplicationDbContext (add your changes quickly and commit)
- Changing Program.cs (use extension methods to minimize conflicts)

**SHOULD coordinate:**
- Before starting work on a new feature (avoid duplicate effort)
- When stuck for more than 30 minutes
- When you need test data from another subsystem

### Slack/Discord Channels (Suggested)

- `#general` - General discussion
- `#identity-user` - Dũng's subsystem questions
- `#conversations` - Nga's subsystem questions
- `#messages-reactions` - Vinh's subsystem questions
- `#realtime-ai` - Hoàn's subsystem questions
- `#integration` - Cross-subsystem integration issues
- `#blockers` - When you're stuck

---

## Resources

### .NET Documentation
- **EF Core:** https://learn.microsoft.com/en-us/ef/core/
- **SignalR:** https://learn.microsoft.com/en-us/aspnet/core/signalr/
- **Blazor:** https://learn.microsoft.com/en-us/aspnet/core/blazor/
- **Identity:** https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

### Code Examples
- **EF Core Relationships:** https://learn.microsoft.com/en-us/ef/core/modeling/relationships
- **SignalR Groups:** https://learn.microsoft.com/en-us/aspnet/core/signalr/groups
- **File Upload:** https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads

### Git
- **Git Branching:** https://git-scm.com/book/en/v2/Git-Branching-Basic-Branching-and-Merging
- **Resolving Conflicts:** https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/addressing-merge-conflicts

---

## FAQ

**Q: Can I modify interfaces?**
A: No, not without team approval. Create a PR with just the interface change and get everyone's approval first.

**Q: What if I need data from another subsystem?**
A: Use the service interface. Inject the service you need (e.g., inject `IUserService` if you need user data).

**Q: My migration conflicts with someone else's. What do I do?**
A: Communicate with the other person. Usually the solution is to merge develop, resolve conflicts, and create a new migration that combines both changes.

**Q: Can I add methods to my interface?**
A: Yes, but create a separate PR for the interface change first. Get approval, merge, then implement in your subsystem.

**Q: Should I write unit tests?**
A: Recommended but not required for MVP. Focus on getting features working first. Tests can be added later.

**Q: How do I test SignalR locally?**
A: Use browser dev tools to monitor WebSocket connections, or create a simple JavaScript client in the browser console.

**Q: What if the AI API is too expensive to test?**
A: Use mock responses during development. Switch to real API calls when ready for production testing.

---

## Success Checklist

By the end of this project, each team member should have:

**Nga:**
- [ ] Working conversation creation (direct and group)
- [ ] Working conversation retrieval
- [ ] Database migrations for conversations
- [ ] Service properly registered

**Vinh:**
- [ ] Working message persistence
- [ ] Working message history with pagination
- [ ] Working reactions (add/update/remove)
- [ ] Working file uploads (images and PDFs)
- [ ] Database migrations for messages, reactions, attachments
- [ ] Services properly registered

**Hoàn:**
- [ ] Working SignalR hub
- [ ] Working realtime message broadcast
- [ ] Working typing indicators
- [ ] Working AI reply suggestions (at least mock)
- [ ] Working story creation and retrieval
- [ ] Working current thought management
- [ ] Database migrations for stories and thoughts
- [ ] Services properly registered

**All together:**
- [ ] Users can register and login (Dũng)
- [ ] Users can create conversations (Nga)
- [ ] Users can send and receive messages in realtime (Vinh + Hoàn)
- [ ] Users can add reactions (Vinh)
- [ ] Users can upload files (Vinh)
- [ ] Users can get AI suggestions (Hoàn)
- [ ] UI integrates all features (Hoàn)

---

**Good luck, team! Remember:**
- Communicate early and often
- Don't change interfaces without approval
- Commit frequently
- Ask for help when stuck
- We're all in this together! 🚀

**Questions?** Contact Dũng (project lead & shared contracts owner)
