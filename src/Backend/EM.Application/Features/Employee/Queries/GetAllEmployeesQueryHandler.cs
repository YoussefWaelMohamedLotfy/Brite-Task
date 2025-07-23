using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Employee.Queries;

public readonly struct GetAllEmployeesQuery : IRequest<IResult>;

internal sealed class GetAllEmployeesQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetAllEmployeesQuery, IResult>
{
    public async Task<IResult> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await dbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .ToListAsync(cancellationToken: cancellationToken);

        return Results.Ok(employees);
    }
}
