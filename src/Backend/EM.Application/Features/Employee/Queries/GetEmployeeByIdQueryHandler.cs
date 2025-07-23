using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Employee.Queries;

public sealed record GetEmployeeByIdQuery(Guid Id) : IRequest<IResult>;

internal sealed class GetEmployeeByIdQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetEmployeeByIdQuery, IResult>
{
    public async Task<IResult> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .FirstOrDefaultAsync(e => e.ID == request.Id, cancellationToken);

        return employee is null ? Results.NotFound() : Results.Ok(employee);
    }
}
