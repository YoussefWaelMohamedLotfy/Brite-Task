using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

public sealed record UpdateRoleCommand(int Id, string Name, List<string> Permissions) : IRequest<IResult>;

internal sealed class UpdateRoleCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<UpdateRoleCommand, IResult>
{
    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);

        if (role is null)
            return Results.NotFound();

        role.Name = request.Name;
        role.Permissions = request.Permissions;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(role);
    }
}
