using EM.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Role.Queries;

/// <summary>
/// Query to retrieve a role by its ID.
/// </summary>
/// <param name="Id">The role ID.</param>
public sealed record GetRoleByIdQuery(int Id) : IRequest<IResult>;

/// <summary>
/// Handles the retrieval of a role by its ID.
/// </summary>
internal sealed class GetRoleByIdQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetRoleByIdQuery, IResult>
{
    /// <summary>
    /// Handles the get role by ID query.
    /// </summary>
    /// <param name="request">The get role by ID query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The role if found, otherwise not found.</returns>
    public async Task<IResult> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await dbContext.Roles.FindAsync([request.Id], cancellationToken);
        return role is not null ? Results.Ok(role) : Results.NotFound();
    }
}
