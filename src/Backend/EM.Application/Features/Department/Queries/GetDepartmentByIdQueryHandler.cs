using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Queries;

public sealed record GetDepartmentByIdQuery(int Id) : IRequest<IResult>;

internal sealed class GetDepartmentByIdQueryHandler(
    AppDbContext dbContext)
    : IRequestHandler<GetDepartmentByIdQuery, IResult>
{
    public async Task<IResult> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments.FindAsync([request.Id], cancellationToken);

        return department is null ? Results.NotFound() : Results.Ok(department);
    }
}
