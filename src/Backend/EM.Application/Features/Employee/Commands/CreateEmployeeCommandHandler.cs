using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

public sealed record CreateEmployeeCommand(
    string Name,
    string Email,
    string Phone,
    DateTimeOffset DateOfJoining,
    bool IsActive,
    int DepartmentId,
    int RoleId,
    Guid CreatedBy)
    : IRequest<IResult>;

internal sealed class CreateEmployeeCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<CreateEmployeeCommand, IResult>
{
    public async Task<IResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments.FindAsync([request.DepartmentId], cancellationToken);
        var role = await dbContext.Roles.FindAsync([request.RoleId], cancellationToken);

        if (department is null || role is null)
            return Results.BadRequest("Invalid Department or Role");

        var employee = new Domain.Entities.Employee
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            DateOfJoining = request.DateOfJoining,
            IsActive = request.IsActive,
            Department = department,
            Role = role,
        };

        await dbContext.Employees.AddAsync(employee, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Created($"/Employees/{employee.ID}", employee);
    }
}
