using AIAgentPlatform.Application.Common.DTOs;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Queries.GetConversationById;

/// <summary>
/// 取得對話詳情 Query Handler
/// </summary>
public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, ConversationDto?>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationByIdQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto?> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.Id, cancellationToken);

        return conversation == null ? null : ConversationDto.FromEntity(conversation);
    }
}
