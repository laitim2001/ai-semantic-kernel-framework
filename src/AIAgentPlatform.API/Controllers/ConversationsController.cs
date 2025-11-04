using AIAgentPlatform.Application.Common.DTOs;
using AIAgentPlatform.Application.Conversations.Commands.AddMessage;
using AIAgentPlatform.Application.Conversations.Commands.CreateConversation;
using AIAgentPlatform.Application.Conversations.Queries.GetConversationById;
using AIAgentPlatform.Application.Conversations.Queries.GetConversations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIAgentPlatform.API.Controllers;

/// <summary>
/// Conversations API Controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ConversationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(
        IMediator mediator,
        ILogger<ConversationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// 取得對話列表
    /// </summary>
    /// <param name="userId">用戶 ID (可選)</param>
    /// <param name="agentId">Agent ID (可選)</param>
    /// <param name="status">狀態 (可選: active, archived, deleted)</param>
    /// <param name="pageNumber">頁碼 (預設: 1)</param>
    /// <param name="pageSize">每頁數量 (預設: 20)</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConversationDto>>> GetConversations(
        [FromQuery] Guid? userId = null,
        [FromQuery] Guid? agentId = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetConversationsQuery
        {
            UserId = userId,
            AgentId = agentId,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var conversations = await _mediator.Send(query);

        return Ok(conversations);
    }

    /// <summary>
    /// 取得對話詳情
    /// </summary>
    /// <param name="id">對話 ID</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDto>> GetConversation(Guid id)
    {
        var query = new GetConversationByIdQuery { Id = id };
        var conversation = await _mediator.Send(query);

        if (conversation == null)
        {
            return NotFound(new { message = $"找不到對話 ID: {id}" });
        }

        return Ok(conversation);
    }

    /// <summary>
    /// 創建對話
    /// </summary>
    /// <param name="command">創建對話請求</param>
    [HttpPost]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConversationDto>> CreateConversation(
        [FromBody] CreateConversationCommand command)
    {
        var conversation = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetConversation),
            new { id = conversation.Id },
            conversation
        );
    }

    /// <summary>
    /// 新增訊息到對話
    /// </summary>
    /// <param name="id">對話 ID</param>
    /// <param name="command">新增訊息請求</param>
    [HttpPost("{id:guid}/messages")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> AddMessage(
        Guid id,
        [FromBody] AddMessageCommand command)
    {
        // Ensure ConversationId matches route parameter
        if (command.ConversationId != id)
        {
            return BadRequest(new { message = "對話 ID 不匹配" });
        }

        try
        {
            var message = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetConversation),
                new { id = message.ConversationId },
                message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增訊息失敗: ConversationId={ConversationId}", id);

            if (ex.Message.Contains("找不到對話"))
            {
                return NotFound(new { message = ex.Message });
            }

            return BadRequest(new { message = ex.Message });
        }
    }
}
