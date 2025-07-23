using EM.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EM.Application.Features.Department.Queries;

public readonly struct GetAllDepartmentsQuery : IRequest<IResult>;

internal sealed class GetAllDepartmentsQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetAllDepartmentsQuery, IResult>
{
    public async Task<IResult> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await dbContext.Departments.ToListAsync(cancellationToken: cancellationToken);
        return Results.Ok(departments);
    }
}
