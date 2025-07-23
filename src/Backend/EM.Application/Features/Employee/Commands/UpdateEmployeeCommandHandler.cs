using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

public sealed record UpdateEmployeeCommand(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateTimeOffset DateOfJoining,
    bool IsActive,
    int DepartmentId,
    int RoleId,
    Guid? UpdatedBy)
    : IRequest<IResult>;

internal sealed class UpdateEmployeeCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<UpdateEmployeeCommand, IResult>
{
    public async Task<IResult> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (employee is null)
            return Results.NotFound();

        var department = await dbContext.Departments.FindAsync([request.DepartmentId], cancellationToken);
        var role = await dbContext.Roles.FindAsync([request.RoleId], cancellationToken);

        if (department is null || role is null)
            return Results.BadRequest("Invalid Department or Role");

        employee.Name = request.Name;
        employee.Email = request.Email;
        employee.Phone = request.Phone;
        employee.DateOfJoining = request.DateOfJoining;
        employee.IsActive = request.IsActive;
        employee.Department = department;
        employee.Role = role;

        dbContext.Employees.Update(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(employee);
    }
}
