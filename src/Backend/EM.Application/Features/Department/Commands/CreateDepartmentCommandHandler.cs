using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

public sealed record CreateDepartmentCommand(string Name, string? Description, Guid CreatedBy) : IRequest<IResult>;

internal sealed class CreateDepartmentCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<CreateDepartmentCommand, IResult>
{
    public async Task<IResult> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = new Domain.Entities.Department
        {
            Name = request.Name,
            Description = request.Description,
        };

        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Created($"/Departments/{department.ID}", department);
    }
}
