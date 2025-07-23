using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Role.Queries;

public readonly struct GetAllRolesQuery : IRequest<IResult>;

internal sealed class GetAllRolesQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetAllRolesQuery, IResult>
{
    public async Task<IResult> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await dbContext.Roles.ToListAsync(cancellationToken: cancellationToken);

        return Results.Ok(roles);
    }
}
