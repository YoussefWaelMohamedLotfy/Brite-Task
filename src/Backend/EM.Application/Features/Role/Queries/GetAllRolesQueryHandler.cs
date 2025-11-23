using EM.Infrastructure.Data;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Role.Queries;

/// <summary>
/// Query to retrieve all roles.
/// </summary>
public readonly struct GetAllRolesQuery : IRequest<IResult>;

/// <summary>
/// Handles the retrieval of all roles.
/// </summary>
public sealed class GetAllRolesQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetAllRolesQuery, IResult>
{
    /// <summary>
    /// Handles the get all roles query.
    /// </summary>
    /// <param name="request">The get all roles query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all roles.</returns>
    public async ValueTask<IResult> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var roles = await dbContext.Roles.ToListAsync(cancellationToken: cancellationToken);

        return Results.Ok(roles);
    }
}
