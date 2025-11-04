using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.ValueObjects;

namespace AIAgentPlatform.Domain.Interfaces;

/// <summary>
/// 對話 Repository 介面
/// </summary>
public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Conversation>> GetAllAsync(
        Guid? userId = null,
        Guid? agentId = null,
        ConversationStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);

    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
