using EM.Infrastructure.Data;

using FluentValidation;

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
    int RoleId)
    : IRequest<IResult>;

internal sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
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
