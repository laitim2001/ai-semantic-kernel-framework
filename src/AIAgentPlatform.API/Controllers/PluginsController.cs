using AIAgentPlatform.API.DTOs.Plugins;
using AIAgentPlatform.Application.Commands.Plugins.ActivatePlugin;
using AIAgentPlatform.Application.Commands.Plugins.DeactivatePlugin;
using AIAgentPlatform.Application.Commands.Plugins.RegisterPlugin;
using AIAgentPlatform.Application.Commands.Plugins.UpdatePlugin;
using AIAgentPlatform.Application.Queries.Plugins.GetPlugin;
using AIAgentPlatform.Application.Queries.Plugins.GetPlugins;
using AIAgentPlatform.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIAgentPlatform.API.Controllers;

/// <summary>
/// Plugin management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PluginsController> _logger;

    public PluginsController(IMediator mediator, ILogger<PluginsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new plugin
    /// </summary>
    /// <param name="request">Plugin registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registered plugin information</returns>
    /// <response code="201">Plugin registered successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="409">Plugin with same ID already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(PluginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PluginResponse>> RegisterPlugin(
        [FromBody] RegisterPluginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new RegisterPluginCommand
            {
                UserId = request.UserId,
                AssemblyPath = request.AssemblyPath,
                PluginId = request.PluginId,
                Name = request.Name,
                Version = request.Version,
                Description = request.Description,
                Category = request.Category,
                AutoActivate = request.AutoActivate
            };

            var plugin = await _mediator.Send(command, cancellationToken);
            var response = MapToResponse(plugin);

            return CreatedAtAction(
                nameof(GetPlugin),
                new { id = plugin.Id },
                response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all plugins with optional filtering and sorting
    /// </summary>
    /// <param name="request">Query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of plugins</returns>
    /// <response code="200">Plugins retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PluginResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PluginResponse>>> GetPlugins(
        [FromQuery] GetPluginsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetPluginsQuery
        {
            UserId = request.UserId,
            Status = request.Status,
            Category = request.Category,
            SearchTerm = request.SearchTerm,
            SortBy = request.SortBy ?? "created",
            SortDirection = request.SortDirection ?? "desc",
            Skip = request.Skip,
            Take = request.Take
        };

        var plugins = await _mediator.Send(query, cancellationToken);
        var responses = plugins.Select(MapToResponse).ToList();

        return Ok(responses);
    }

    /// <summary>
    /// Get a specific plugin by ID
    /// </summary>
    /// <param name="id">Plugin ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Plugin information</returns>
    /// <response code="200">Plugin found</response>
    /// <response code="404">Plugin not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PluginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PluginResponse>> GetPlugin(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPluginQuery(id);
        var plugin = await _mediator.Send(query, cancellationToken);

        if (plugin == null)
        {
            return NotFound(new { error = $"Plugin with ID '{id}' not found" });
        }

        return Ok(MapToResponse(plugin));
    }

    /// <summary>
    /// Update plugin information
    /// </summary>
    /// <param name="id">Plugin ID</param>
    /// <param name="request">Updated plugin information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Plugin updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Plugin not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePlugin(
        Guid id,
        [FromBody] UpdatePluginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdatePluginCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Category = request.Category
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Activate a plugin
    /// </summary>
    /// <param name="id">Plugin ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Plugin activated successfully</response>
    /// <response code="404">Plugin not found</response>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivatePlugin(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new ActivatePluginCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate a plugin
    /// </summary>
    /// <param name="id">Plugin ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Plugin deactivated successfully</response>
    /// <response code="404">Plugin not found</response>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivatePlugin(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeactivatePluginCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    private static PluginResponse MapToResponse(Plugin plugin)
    {
        return new PluginResponse
        {
            Id = plugin.Id,
            UserId = plugin.UserId,
            PluginId = plugin.PluginId,
            Name = plugin.Name,
            Description = plugin.Description,
            Version = plugin.Version,
            Category = plugin.Category,
            Status = plugin.Status.ToString(),
            Functions = plugin.Metadata.Functions.Select(f => new PluginFunctionResponse
            {
                Name = f.Name,
                Description = f.Description,
                ReturnType = f.ReturnType ?? "void",
                Parameters = f.Parameters.Select(p => new PluginParameterResponse
                {
                    Name = p.Name,
                    Type = p.Type,
                    Description = p.Description,
                    IsRequired = p.IsRequired
                }).ToList()
            }).ToList(),
            CreatedAt = plugin.CreatedAt,
            UpdatedAt = plugin.UpdatedAt
        };
    }
}
