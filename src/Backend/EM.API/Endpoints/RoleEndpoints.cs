using EM.Application.Features.Role.Commands;
using EM.Application.Features.Role.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MinimalApis.Discovery;

namespace EM.API.Endpoints;

public sealed class RoleEndpoints : IApi
{
    public void Register(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/roles")
            .WithTags("Roles")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetAllRoles)
            .WithName("GetRoles")
            .WithSummary("Retrieves a list of roles")
            .WithDescription("This endpoint returns a list of all available roles in the system.")
            .CacheOutput()
            .Produces<string>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetRoleById)
            .WithName("GetRoleById")
            .WithSummary("Retrieves a role by ID")
            .WithDescription("This endpoint returns a single role based on its ID in the system.")
            .CacheOutput();

        group.MapPost("/", CreateRole)
            .WithName("CreateRole")
            .WithSummary("Creates a new role")
            .WithDescription("This endpoint allows the creation of a new role in the system.")
            .Produces<string>(StatusCodes.Status201Created);

        group.MapPut("/", UpdateRole)
            .WithName("UpdateRole")
            .WithSummary("Updates an existing role")
            .WithDescription("This endpoint allows updating an existing role.")
            .Produces(StatusCodes.Status200OK);

        group.MapDelete("/{id}", DeleteRole)
            .WithName("DeleteRole")
            .WithSummary("Deletes a role")
            .WithDescription("This endpoint allows deleting a role.")
            .Produces(StatusCodes.Status204NoContent);
    }

    public static async Task<IResult> GetAllRoles(IMediator mediator, CancellationToken ct)
        => await mediator.Send(new GetAllRolesQuery(), ct);

    public static async Task<IResult> GetRoleById(int id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new GetRoleByIdQuery(id), ct);

    public static async Task<IResult> CreateRole([FromBody] CreateRoleCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> UpdateRole([FromBody] UpdateRoleCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> DeleteRole(int id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new DeleteRoleCommand(id), ct);
}
