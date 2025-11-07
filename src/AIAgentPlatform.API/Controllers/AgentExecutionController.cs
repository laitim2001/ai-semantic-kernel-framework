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
    /// Get execution history for an agent with advanced filtering, sorting, and pagination
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="status">Filter by status (running, completed, failed, cancelled)</param>
    /// <param name="conversationId">Filter by conversation ID</param>
    /// <param name="minTokens">Minimum tokens used</param>
    /// <param name="maxTokens">Maximum tokens used</param>
    /// <param name="minResponseTimeMs">Minimum response time in milliseconds</param>
    /// <param name="maxResponseTimeMs">Maximum response time in milliseconds</param>
    /// <param name="searchTerm">Search term (searches in error messages and metadata)</param>
    /// <param name="sortBy">Sort by field (startTime, endTime, responseTimeMs, tokensUsed, status)</param>
    /// <param name="sortDescending">Sort descending (default: true)</param>
    /// <param name="skip">Number of records to skip (default: 0)</param>
    /// <param name="take">Number of records to take (default: 50, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("history")]
    [ProducesResponseType(typeof(PagedResultDto<AgentExecutionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<AgentExecutionDto>>> GetHistory(
        Guid agentId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? status,
        [FromQuery] Guid? conversationId,
        [FromQuery] int? minTokens,
        [FromQuery] int? maxTokens,
        [FromQuery] double? minResponseTimeMs,
        [FromQuery] double? maxResponseTimeMs,
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDescending = true,
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
            ConversationId = conversationId,
            MinTokens = minTokens,
            MaxTokens = maxTokens,
            MinResponseTimeMs = minResponseTimeMs,
            MaxResponseTimeMs = maxResponseTimeMs,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Skip = skip,
            Take = Math.Min(take, 100) // Max 100 records
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get execution statistics for an agent with enhanced performance metrics
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="startDate">Filter by start date</param>
    /// <param name="endDate">Filter by end date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed execution statistics including response time percentiles and token usage</returns>
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

    /// <summary>
    /// Get time-series execution statistics for an agent
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="startDate">Start date for time series</param>
    /// <param name="endDate">End date for time series</param>
    /// <param name="granularity">Time granularity: hour, day, week, month (default: day)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Time-series statistics grouped by specified granularity</returns>
    [HttpGet("statistics/timeseries")]
    [ProducesResponseType(typeof(TimeSeriesStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TimeSeriesStatisticsDto>> GetTimeSeriesStatistics(
        Guid agentId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string granularity = "day",
        CancellationToken cancellationToken = default)
    {
        if (startDate >= endDate)
        {
            return BadRequest("Start date must be before end date");
        }

        var query = new GetTimeSeriesStatistics
        {
            AgentId = agentId,
            StartDate = startDate,
            EndDate = endDate,
            Granularity = granularity
        };

        try
        {
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get detailed execution information including navigation properties
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="executionId">The execution ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{executionId:guid}")]
    [ProducesResponseType(typeof(AgentExecutionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentExecutionDetailDto>> GetExecutionDetails(
        Guid agentId,
        Guid executionId,
        CancellationToken cancellationToken)
    {
        var query = new GetExecutionDetails
        {
            ExecutionId = executionId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result == null)
        {
            return NotFound($"Execution {executionId} not found");
        }

        // Verify the execution belongs to this agent
        if (result.AgentId != agentId)
        {
            return NotFound($"Execution {executionId} does not belong to agent {agentId}");
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all executions for a specific conversation
    /// </summary>
    /// <param name="agentId">The agent ID</param>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("conversations/{conversationId:guid}/executions")]
    [ProducesResponseType(typeof(List<AgentExecutionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AgentExecutionDto>>> GetConversationExecutions(
        Guid agentId,
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        var query = new GetConversationExecutions
        {
            ConversationId = conversationId
        };

        var result = await _sender.Send(query, cancellationToken);

        // Filter to only include executions for this agent
        var agentExecutions = result.Where(e => e.AgentId == agentId).ToList();

        return Ok(agentExecutions);
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
