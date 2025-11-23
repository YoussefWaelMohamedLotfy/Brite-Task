using EM.Infrastructure.Data;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Employee.Queries;

/// <summary>
/// Query to retrieve all employees with optional search criteria.
/// </summary>
public sealed record GetAllEmployeesQuery(
    string? Name,
    int? DepartmentId,
    bool? IsActive,
    DateTimeOffset? DateOfJoiningFrom,
    DateTimeOffset? DateOfJoiningTo
) : IRequest<IResult>;

/// <summary>
/// Handles the retrieval of all employees.
/// </summary>
public sealed class GetAllEmployeesQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetAllEmployeesQuery, IResult>
{
    /// <summary>
    /// Handles the get all employees query.
    /// </summary>
    /// <param name="request">The get all employees query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all employees.</returns>
    public async ValueTask<IResult> Handle(
        GetAllEmployeesQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext
            .Employees.Include(e => e.Department)
            .Include(e => e.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(e => e.Name.Contains(request.Name));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(e =>
                e.Department != null && e.Department.ID == request.DepartmentId.Value
            );
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == request.IsActive.Value);
        }

        if (request.DateOfJoiningFrom.HasValue)
        {
            query = query.Where(e => e.DateOfJoining >= request.DateOfJoiningFrom.Value);
        }

        if (request.DateOfJoiningTo.HasValue)
        {
            query = query.Where(e => e.DateOfJoining <= request.DateOfJoiningTo.Value);
        }

        var employees = await query.ToListAsync(cancellationToken: cancellationToken);
        return Results.Ok(employees);
    }
}
