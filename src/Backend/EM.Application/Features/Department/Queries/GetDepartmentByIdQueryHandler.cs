using EM.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Queries;

/// <summary>
/// Query to retrieve a department by its ID.
/// </summary>
/// <param name="Id">The department ID.</param>
public sealed record GetDepartmentByIdQuery(int Id) : IRequest<IResult>;

/// <summary>
/// Handles the retrieval of a department by its ID.
/// </summary>
internal sealed class GetDepartmentByIdQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetDepartmentByIdQuery, IResult>
{
    /// <summary>
    /// Handles the get department by ID query.
    /// </summary>
    /// <param name="request">The get department by ID query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The department if found, otherwise not found.</returns>
    public async Task<IResult> Handle(
        GetDepartmentByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var department = await dbContext.Departments.FindAsync([request.Id], cancellationToken);

        return department is null ? Results.NotFound() : Results.Ok(department);
    }
}
