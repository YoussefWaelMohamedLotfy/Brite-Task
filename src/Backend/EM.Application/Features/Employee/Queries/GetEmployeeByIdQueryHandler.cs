using EM.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Employee.Queries;

/// <summary>
/// Query to retrieve an employee by their ID.
/// </summary>
/// <param name="Id">The employee ID.</param>
public sealed record GetEmployeeByIdQuery(Guid Id) : IRequest<IResult>;

/// <summary>
/// Handles the retrieval of an employee by their ID.
/// </summary>
internal sealed class GetEmployeeByIdQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetEmployeeByIdQuery, IResult>
{
    /// <summary>
    /// Handles the get employee by ID query.
    /// </summary>
    /// <param name="request">The get employee by ID query.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The employee if found, otherwise not found.</returns>
    public async Task<IResult> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var employee = await dbContext
            .Employees.Include(e => e.Department)
            .Include(e => e.Role)
            .FirstOrDefaultAsync(e => e.ID == request.Id, cancellationToken);

        return employee is null ? Results.NotFound() : Results.Ok(employee);
    }
}
