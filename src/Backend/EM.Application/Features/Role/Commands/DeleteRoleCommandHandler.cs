using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Commands;

public sealed record DeleteRoleCommand(int Id) : IRequest<IResult>;

internal sealed class DeleteRoleCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<DeleteRoleCommand, IResult>
{
    public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);

        if (role is null)
            return Results.NotFound();

        dbContext.Roles.Remove(role);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}
