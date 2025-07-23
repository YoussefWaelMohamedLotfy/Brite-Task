using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

public sealed record CreateRoleCommand(string Name, List<string> Permissions) : IRequest<IResult>;

internal sealed class CreateRoleCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<CreateRoleCommand, IResult>
{
    public async Task<IResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new Domain.Entities.Role
        {
            Name = request.Name,
            Permissions = request.Permissions,
        };

        await dbContext.Roles.AddAsync(role, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Created($"/Roles/{role.ID}", role);
    }
}
