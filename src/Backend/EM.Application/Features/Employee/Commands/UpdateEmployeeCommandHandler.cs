using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

/// <summary>
/// Command to update an existing employee.
/// </summary>
/// <param name="Id">The employee ID.</param>
/// <param name="Name">The new name of the employee.</param>
/// <param name="Email">The new email address of the employee.</param>
/// <param name="Phone">The new phone number of the employee.</param>
/// <param name="DateOfJoining">The new date of joining.</param>
/// <param name="IsActive">Whether the employee is active.</param>
/// <param name="DepartmentId">The new department ID.</param>
/// <param name="RoleId">The new role ID.</param>
public sealed record UpdateEmployeeCommand(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    DateTimeOffset DateOfJoining,
    bool IsActive,
    int DepartmentId,
    int RoleId
) : ICommand<IResult>;

/// <summary>
/// Handles the update of an existing employee.
/// </summary>
internal sealed class UpdateEmployeeCommandHandler(AppDbContext dbContext)
    : ICommandHandler<UpdateEmployeeCommand, IResult>
{
    /// <summary>
    /// Handles the update employee command.
    /// </summary>
    /// <param name="request">The update employee command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the update operation.</returns>
    public async Task<IResult> Handle(
        UpdateEmployeeCommand request,
        CancellationToken cancellationToken
    )
    {
        var employee = await dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (employee is null)
            return Results.NotFound();

        var department = await dbContext.Departments.FindAsync(
            [request.DepartmentId],
            cancellationToken
        );
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

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(employee);
    }
}

/// <summary>
/// Validator for <see cref="UpdateEmployeeCommand"/>.
/// </summary>
internal sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateEmployeeCommandValidator"/> class.
    /// </summary>
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.DateOfJoining)
            .LessThanOrEqualTo(DateTimeOffset.Now)
            .WithMessage("Date of joining cannot be in the future.");
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("Invalid Department ID.");
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Invalid Role ID.");
    }
}
