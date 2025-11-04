using AIAgentPlatform.Application.Common.DTOs;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Interfaces;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Commands.CreateConversation;

/// <summary>
/// 創建對話 Command Handler
/// </summary>
public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    private readonly IConversationRepository _conversationRepository;

    public CreateConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        // 創建對話實體
        var conversation = Conversation.Create(
            request.AgentId,
            request.UserId,
            request.Title
        );

        // 保存到數據庫
        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _conversationRepository.SaveChangesAsync(cancellationToken);

        // 返回 DTO
        return ConversationDto.FromEntity(conversation);
    }
}
