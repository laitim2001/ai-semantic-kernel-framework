using AIAgentPlatform.Application.Common.DTOs;
using AIAgentPlatform.Domain.Entities;
using AIAgentPlatform.Domain.Exceptions;
using AIAgentPlatform.Domain.Interfaces;
using AIAgentPlatform.Domain.ValueObjects;
using MediatR;

namespace AIAgentPlatform.Application.Conversations.Commands.AddMessage;

/// <summary>
/// 新增訊息 Command Handler
/// </summary>
public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, MessageDto>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public AddMessageCommandHandler(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        // 取得對話
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
        {
            throw new EntityNotFoundException($"找不到對話 ID: {request.ConversationId}");
        }

        // 創建訊息
        var role = MessageRole.Create(request.Role);
        var message = Message.Create(
            request.ConversationId,
            role,
            request.Content
        );

        // 將訊息加入對話
        conversation.AddMessage(message);

        // 保存
        await _messageRepository.AddAsync(message, cancellationToken);
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _conversationRepository.SaveChangesAsync(cancellationToken);

        return MessageDto.FromEntity(message);
    }
}
