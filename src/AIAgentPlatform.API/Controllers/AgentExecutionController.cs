using AIAgentPlatform.Application.AgentExecutions.Commands;
using AIAgentPlatform.Application.AgentExecutions.DTOs;
using AIAgentPlatform.Application.AgentExecutions.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIAgentPlatform.API.Controllers;

/// <summary>
/// API endpoints for Agent execution and monitoring
/// </summary>
[ApiController]
[Route("api/agents/{agentId:guid}/[controller]")]
[Produces("application/json")]
public sealed class AgentExecutionController : ControllerBase
{
    private readonly ISender _sender;

    public AgentExecutionController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Execute an agent with the given input
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="request">Execution request containing conversation ID and input</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("execute")]
    [ProducesResponseType(typeof(AgentExecutionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentExecutionResultDto>> Execute(
        Guid agentId,
        [FromBody] ExecuteAgentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Input))
        {
            return BadRequest("Input cannot be empty");
        }

        var command = new ExecuteAgentCommand
        {
            AgentId = agentId,
            ConversationId = request.ConversationId,
            Input = request.Input,
            Metadata = request.Metadata
        };

        try
        {
            var result = await _sender.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get execution history for an agent
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="status">Filter by status (running, completed, failed, cancelled)</param>
    /// <param name="skip">Number of records to skip (default: 0)</param>
    /// <param name="take">Number of records to take (default: 50, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<AgentExecutionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AgentExecutionDto>>> GetHistory(
        Guid agentId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? status,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetExecutionHistory
        {
            AgentId = agentId,
            StartDate = startDate,
            EndDate = endDate,
            Status = status,
            Skip = skip,
            Take = Math.Min(take, 100) // Max 100 records
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get execution statistics for an agent
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(AgentExecutionStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AgentExecutionStatisticsDto>> GetStatistics(
        Guid agentId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAgentExecutionStatistics
        {
            AgentId = agentId,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }
}

/// <summary>
/// Request model for executing an agent
/// </summary>
public sealed class ExecuteAgentRequest
{
    public Guid ConversationId { get; init; }
    public string Input { get; init; } = string.Empty;
    public string? Metadata { get; init; }
}
