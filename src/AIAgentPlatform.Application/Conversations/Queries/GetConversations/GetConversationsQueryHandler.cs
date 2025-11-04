using AIAgentPlatform.Application.Common.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Queries.GetConversations;

/// <summary>
/// 取得對話列表 Query Handler
/// </summary>
public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationsQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<List<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        ConversationStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            status = ConversationStatus.Create(request.Status);
        }

        var conversations = await _conversationRepository.GetAllAsync(
            request.UserId,
            request.AgentId,
            status,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        return conversations.Select(ConversationDto.FromEntity).ToList();
    }
}
