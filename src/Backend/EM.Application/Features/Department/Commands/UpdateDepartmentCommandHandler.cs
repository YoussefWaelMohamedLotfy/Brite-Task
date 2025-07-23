using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

public sealed record UpdateDepartmentCommand(int Id, string Name, string? Description, Guid? UpdatedBy) : IRequest<IResult>;

internal sealed class UpdateDepartmentCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<UpdateDepartmentCommand, IResult>
{
    public async Task<IResult> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments.FindAsync([request.Id], cancellationToken);

        if (department is null)
            return Results.NotFound();

        department.Name = request.Name;
        department.Description = request.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(department);
    }
}
