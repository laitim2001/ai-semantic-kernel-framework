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
    /// Get a list of agents with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAgentsQueryResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAgentsQueryResult>> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] string? status,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAgentsQuery
        {
            UserId = userId,
            Status = status,
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
}
