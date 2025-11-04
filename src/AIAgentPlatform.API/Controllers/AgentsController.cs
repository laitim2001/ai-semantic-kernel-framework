using AIAgentPlatform.Application.Agents.Commands;
using AIAgentPlatform.Application.Agents.DTOs;
using AIAgentPlatform.Application.Agents.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIAgentPlatform.API.Controllers;

/// <summary>
/// API endpoints for Agent management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AgentsController : ControllerBase
{
    private readonly ISender _sender;

    public AgentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a single agent by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAgentByIdQuery { Id = id };
        var result = await _sender.Send(query, cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Get a list of agents with pagination, filtering, searching, and sorting
    /// </summary>
    /// <param name="userId">Filter by user ID</param>
    /// <param name="status">Filter by status (active, paused, archived)</param>
    /// <param name="searchTerm">Search in name, description, or ID</param>
    /// <param name="model">Filter by model name (e.g., gpt-4, gpt-4o)</param>
    /// <param name="sortBy">Sort field: name, createdAt, updatedAt (default: createdAt)</param>
    /// <param name="sortOrder">Sort order: asc, desc (default: desc)</param>
    /// <param name="skip">Number of records to skip (default: 0)</param>
    /// <param name="take">Number of records to take (default: 50, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(GetAgentsQueryResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAgentsQueryResult>> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] string? status,
        [FromQuery] string? searchTerm,
        [FromQuery] string? model,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAgentsQuery
        {
            UserId = userId,
            Status = status,
            SearchTerm = searchTerm,
            Model = model,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Skip = skip,
            Take = take
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new agent
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AgentDto>> Create(
        [FromBody] CreateAgentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing agent
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentDto>> Update(
        Guid id,
        [FromBody] UpdateAgentCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL does not match ID in request body");
        }

        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete an agent (archive/soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAgentCommand { Id = id };
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Activate an agent (change status to Active)
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentDto>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateAgentCommand { Id = id };
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Pause an agent (change status to Paused)
    /// </summary>
    [HttpPost("{id:guid}/pause")]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentDto>> Pause(Guid id, CancellationToken cancellationToken)
    {
        var command = new PauseAgentCommand { Id = id };
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Archive an agent (change status to Archived - soft delete)
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(typeof(AgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentDto>> Archive(Guid id, CancellationToken cancellationToken)
    {
        var command = new ArchiveAgentCommand { Id = id };
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    // ========== Batch Operations ==========

    /// <summary>
    /// Batch activate multiple agents
    /// </summary>
    /// <param name="command">Command containing list of agent IDs to activate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("batch/activate")]
    [ProducesResponseType(typeof(BatchOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BatchOperationResult>> BatchActivate(
        [FromBody] BatchActivateAgentsCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Batch pause multiple agents
    /// </summary>
    /// <param name="command">Command containing list of agent IDs to pause</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("batch/pause")]
    [ProducesResponseType(typeof(BatchOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BatchOperationResult>> BatchPause(
        [FromBody] BatchPauseAgentsCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Batch archive multiple agents
    /// </summary>
    /// <param name="command">Command containing list of agent IDs to archive</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("batch/archive")]
    [ProducesResponseType(typeof(BatchOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BatchOperationResult>> BatchArchive(
        [FromBody] BatchArchiveAgentsCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Batch delete multiple agents (soft delete/archive)
    /// </summary>
    /// <param name="command">Command containing list of agent IDs to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("batch/delete")]
    [ProducesResponseType(typeof(BatchOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BatchOperationResult>> BatchDelete(
        [FromBody] BatchDeleteAgentsCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}
