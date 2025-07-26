using EM.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Department.Queries;

/// <summary>
/// Query to retrieve all departments.
/// </summary>
public readonly struct GetAllDepartmentsQuery : IRequest<IResult>;

/// <summary>
/// Handles the retrieval of all departments.
/// </summary>
internal sealed class GetAllDepartmentsQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetAllDepartmentsQuery, IResult>
{
    /// <summary>
    /// Handles the get all departments query.
    /// </summary>
    /// <param name="request">The get all departments query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all departments.</returns>
    public async Task<IResult> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await dbContext.Departments.ToListAsync(cancellationToken);
        return Results.Ok(departments);
    }
}
