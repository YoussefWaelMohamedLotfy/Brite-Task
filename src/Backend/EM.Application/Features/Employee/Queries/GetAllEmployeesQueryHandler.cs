using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Employee.Queries;

/// <summary>
/// Query to retrieve all employees.
/// </summary>
public readonly struct GetAllEmployeesQuery : IRequest<IResult>;

/// <summary>
/// Handles the retrieval of all employees.
/// </summary>
internal sealed class GetAllEmployeesQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetAllEmployeesQuery, IResult>
{
    /// <summary>
    /// Handles the get all employees query.
    /// </summary>
    /// <param name="request">The get all employees query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all employees.</returns>
    public async Task<IResult> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await dbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .ToListAsync(cancellationToken: cancellationToken);

        return Results.Ok(employees);
    }
}
