using B2Connect.Admin.MCP.Data;
using B2Connect.Shared.Tenancy.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Admin.MCP.Services;

/// <summary>
/// Service for managing conversation memory and context persistence
/// </summary>
public class ConversationService
{
    private readonly McpDbContext _dbContext;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(
        McpDbContext dbContext,
        TenantContext tenantContext,
        ILogger<ConversationService> logger)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Create a new conversation
    /// </summary>
    public async Task<Conversation> CreateConversationAsync(string userId, string title, string? currentPage = null)
    {
        var conversation = new Conversation
        {
            TenantId = _tenantContext.TenantId,
            UserId = userId,
            Title = title,
            CurrentPage = currentPage,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Conversations.Add(conversation);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Created new conversation {ConversationId} for user {UserId}", conversation.Id, userId);
        return conversation;
    }

    /// <summary>
    /// Get conversation by ID
    /// </summary>
    public async Task<Conversation?> GetConversationAsync(int conversationId)
    {
        return await _dbContext.Conversations
            .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
            .Include(c => c.Context)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.TenantId == _tenantContext.TenantId);
    }

    /// <summary>
    /// Get active conversations for a user
    /// </summary>
    public async Task<List<Conversation>> GetUserConversationsAsync(string userId, int limit = 10)
    {
        return await _dbContext.Conversations
            .Where(c => c.TenantId == _tenantContext.TenantId &&
                       c.UserId == userId &&
                       c.IsActive)
            .OrderByDescending(c => c.UpdatedAt)
            .Take(limit)
            .ToListAsync();
    }

    /// <summary>
    /// Add a message to a conversation
    /// </summary>
    public async Task<ConversationMessage> AddMessageAsync(int conversationId, string sender, string content,
        string? toolName = null, string? toolArgs = null, bool isError = false)
    {
        var message = new ConversationMessage
        {
            ConversationId = conversationId,
            Sender = sender,
            Content = content,
            ToolName = toolName,
            ToolArgs = toolArgs,
            IsError = isError,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ConversationMessages.Add(message);

        // Update conversation timestamp
        var conversation = await _dbContext.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Added {Sender} message to conversation {ConversationId}", sender, conversationId);
        return message;
    }

    /// <summary>
    /// Set conversation context
    /// </summary>
    public async Task SetContextAsync(int conversationId, string key, string value)
    {
        var context = await _dbContext.ConversationContexts
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.Key == key);

        if (context == null)
        {
            context = new ConversationContext
            {
                ConversationId = conversationId,
                Key = key,
                Value = value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.ConversationContexts.Add(context);
        }
        else
        {
            context.Value = value;
            context.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get conversation context
    /// </summary>
    public async Task<Dictionary<string, string>> GetContextAsync(int conversationId)
    {
        var contexts = await _dbContext.ConversationContexts
            .Where(c => c.ConversationId == conversationId)
            .ToListAsync();

        return contexts.ToDictionary(c => c.Key, c => c.Value);
    }

    /// <summary>
    /// Update conversation title based on first user message
    /// </summary>
    public async Task UpdateConversationTitleAsync(int conversationId, string title)
    {
        var conversation = await _dbContext.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.Title = title;
            conversation.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Archive a conversation
    /// </summary>
    public async Task ArchiveConversationAsync(int conversationId)
    {
        var conversation = await _dbContext.Conversations.FindAsync(conversationId);
        if (conversation != null && conversation.TenantId == _tenantContext.TenantId)
        {
            conversation.IsActive = false;
            conversation.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Archived conversation {ConversationId}", conversationId);
        }
    }

    /// <summary>
    /// Get conversation history for context (last N messages)
    /// </summary>
    public async Task<List<ConversationMessage>> GetConversationHistoryAsync(int conversationId, int limit = 10)
    {
        return await _dbContext.ConversationMessages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Clean up old conversations (older than specified days)
    /// </summary>
    public async Task<int> CleanupOldConversationsAsync(int daysOld)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);

        var oldConversations = await _dbContext.Conversations
            .Where(c => c.TenantId == _tenantContext.TenantId &&
                       c.UpdatedAt < cutoffDate &&
                       !c.IsActive)
            .ToListAsync();

        if (oldConversations.Any())
        {
            _dbContext.Conversations.RemoveRange(oldConversations);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} old conversations for tenant {TenantId}",
                oldConversations.Count, _tenantContext.TenantId);
        }

        return oldConversations.Count;
    }
}