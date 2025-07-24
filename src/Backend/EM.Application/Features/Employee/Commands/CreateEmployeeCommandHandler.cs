using EM.Infrastructure.Data;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

/// <summary>
/// Command to create a new employee.
/// </summary>
/// <param name="Name">The name of the employee.</param>
/// <param name="Email">The email address of the employee.</param>
/// <param name="Phone">The phone number of the employee.</param>
/// <param name="DateOfJoining">The date the employee joined.</param>
/// <param name="IsActive">Whether the employee is active.</param>
/// <param name="DepartmentId">The department ID.</param>
/// <param name="RoleId">The role ID.</param>
public sealed record CreateEmployeeCommand(
    string Name,
    string Email,
    string Phone,
    DateTimeOffset DateOfJoining,
    bool IsActive,
    int DepartmentId,
    int RoleId)
    : IRequest<IResult>;

/// <summary>
/// Validator for <see cref="CreateEmployeeCommand"/>.
/// </summary>
internal sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateEmployeeCommandValidator"/> class.
    /// </summary>
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number is required.");
        RuleFor(x => x.DateOfJoining).LessThanOrEqualTo(DateTimeOffset.Now)
            .WithMessage("Date of joining cannot be in the future.");
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("Invalid Department ID.");
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Invalid Role ID.");
    }
}

/// <summary>
/// Handles the creation of a new employee.
/// </summary>
internal sealed class CreateEmployeeCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<CreateEmployeeCommand, IResult>
{
    /// <summary>
    /// Handles the create employee command.
    /// </summary>
    /// <param name="request">The create employee command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the creation operation.</returns>
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
