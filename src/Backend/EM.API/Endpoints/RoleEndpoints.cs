using EM.Application.Features.Role.Commands;
using EM.Application.Features.Role.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinimalApis.Discovery;

namespace EM.API.Endpoints;

/// <summary>
/// API endpoints for managing roles.
/// </summary>
public sealed class RoleEndpoints : IApi
{
    /// <summary>
    /// Registers role endpoints to the route builder.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public void Register(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/roles").WithTags("Roles").WithOpenApi();

        group
            .MapGet("/", GetAllRoles)
            .WithName("GetRoles")
            .WithSummary("Retrieves a list of roles")
            .WithDescription("This endpoint returns a list of all available roles in the system.")
            .CacheOutput()
            .Produces<IEnumerable<Domain.Entities.Role>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("Viewer-Admin-Policy");

        group
            .MapGet("/{id}", GetRoleById)
            .WithName("GetRoleById")
            .WithSummary("Retrieves a role by ID")
            .WithDescription("This endpoint returns a single role based on its ID in the system.")
            .CacheOutput()
            .Produces<Domain.Entities.Role>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("Viewer-Admin-Policy");

        group
            .MapPost("/", CreateRole)
            .WithName("CreateRole")
            .WithSummary("Creates a new role")
            .WithDescription("This endpoint allows the creation of a new role in the system.")
            .Produces<Domain.Entities.Role>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("Admin-Policy");

        group
            .MapPut("/", UpdateRole)
            .WithName("UpdateRole")
            .WithSummary("Updates an existing role")
            .WithDescription("This endpoint allows updating an existing role.")
            .Produces<Domain.Entities.Role>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem()
            .RequireAuthorization("Admin-Policy");

        group
            .MapDelete("/{id}", DeleteRole)
            .WithName("DeleteRole")
            .WithSummary("Deletes a role")
            .WithDescription("This endpoint allows deleting a role.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization("Admin-Policy");
    }

    /// <summary>
    /// Retrieves all roles.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A list of roles.</returns>
    public static async Task<IResult> GetAllRoles(IMediator mediator, CancellationToken ct) =>
        await mediator.Send(new GetAllRolesQuery(), ct);

    /// <summary>
    /// Retrieves a role by its ID.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The role if found.</returns>
    public static async Task<IResult> GetRoleById(
        int id,
        IMediator mediator,
        CancellationToken ct
    ) => await mediator.Send(new GetRoleByIdQuery(id), ct);

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="request">The create role command.</param>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The result of the creation operation.</returns>
    public static async Task<IResult> CreateRole(
        [FromBody] CreateRoleCommand request,
        IMediator mediator,
        CancellationToken ct
    ) => await mediator.Send(request, ct);

    public static async Task<IResult> UpdateRole(
        [FromBody] UpdateRoleCommand request,
        IMediator mediator,
        CancellationToken ct
    ) => await mediator.Send(request, ct);

    public static async Task<IResult> DeleteRole(
        int id,
        IMediator mediator,
        CancellationToken ct
    ) => await mediator.Send(new DeleteRoleCommand(id), ct);
}
