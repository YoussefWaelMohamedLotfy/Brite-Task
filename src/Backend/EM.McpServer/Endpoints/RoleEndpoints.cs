using System.ComponentModel;

using EM.Application.Features.Role.Commands;
using EM.Application.Features.Role.Queries;

using MediatR;

using ModelContextProtocol.Server;

namespace EM.McpServer.Endpoints;

[McpServerToolType]
public sealed class RoleEndpoints(IMediator mediator)
{
    [McpServerTool(Name = nameof(GetAllRoles))]
    [Description("Gets all roles in the Employee Management System")]
    public async Task<IResult> GetAllRoles(CancellationToken ct)
    {
        return await mediator.Send(new GetAllRolesQuery(), ct);
    }

    [McpServerTool(Name = nameof(GetRoleById))]
    [Description("Gets a role by its ID")]
    public async Task<IResult> GetRoleById(
        [Description("The role ID")] int id,
        CancellationToken ct)
    {
        return await mediator.Send(new GetRoleByIdQuery(id), ct);
    }

    [McpServerTool(Name = nameof(CreateRole))]
    [Description("Creates a new role")]
    public async Task<IResult> CreateRole(
        [Description("Role name")] string name,
        [Description("Role permissions")] List<string> permissions,
        CancellationToken ct)
    {
        var command = new CreateRoleCommand(name, permissions);
        return await mediator.Send(command, ct);
    }

    [McpServerTool(Name = nameof(UpdateRole))]
    [Description("Updates an existing role")]
    public async Task<IResult> UpdateRole(
        [Description("Role ID")] int id,
        [Description("Role name")] string name,
        [Description("Role permissions")] List<string> permissions,
        CancellationToken ct)
    {
        var command = new UpdateRoleCommand(id, name, permissions);
        return await mediator.Send(command, ct);
    }

    [McpServerTool(Name = nameof(DeleteRole))]
    [Description("Deletes a role")]
    public async Task<IResult> DeleteRole(
        [Description("The role ID")] int id,
        CancellationToken ct)
    {
        return await mediator.Send(new DeleteRoleCommand(id), ct);
    }
}
