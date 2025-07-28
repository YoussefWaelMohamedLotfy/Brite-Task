using System.ComponentModel;

using EM.Application.Features.Role.Queries;
using EM.Domain.Entities;

using MediatR;

using ModelContextProtocol.Server;

namespace EM.McpServer.Endpoints;

[McpServerToolType]
public sealed class RoleEndpoints(IMediator mediator)
{
    [McpServerTool()]
    [Description("Gets all roles in the system")]
    public async Task<IResult> GetAllRoles(CancellationToken ct)
    {
        return await mediator.Send(new GetAllRolesQuery(), ct);
    }
}
