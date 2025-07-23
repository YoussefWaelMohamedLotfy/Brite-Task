using EM.Infrastructure.Data;
using EM.Application.Features.Common.Abstractions;

using MediatR;

using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace EM.Application.Features.Employee.Commands;

public sealed record UpdateEmployeeCommand(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateTimeOffset DateOfJoining,
    bool IsActive,
    int DepartmentId,
    int RoleId)
    : ICommand<IResult>;

internal sealed class UpdateEmployeeCommandHandler(
    AppDbContext dbContext)
    : ICommandHandler<UpdateEmployeeCommand, IResult>
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

internal sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.DateOfJoining).LessThanOrEqualTo(DateTimeOffset.Now)
            .WithMessage("Date of joining cannot be in the future.");
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("Invalid Department ID.");
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Invalid Role ID.");
    }
}
