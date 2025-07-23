using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Queries;

public sealed record GetRoleByIdQuery(int Id) : IRequest<IResult>;

internal sealed class GetRoleByIdQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetRoleByIdQuery, IResult>
{
    public async Task<IResult> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);
        return role is not null ? Results.Ok(role) : Results.NotFound();
    }
}
