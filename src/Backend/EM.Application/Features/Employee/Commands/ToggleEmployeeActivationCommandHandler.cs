using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

/// <summary>
/// Command to toggle the activation status of an employee by ID.
/// </summary>
/// <param name="Id">The employee ID.</param>
public sealed record ToggleEmployeeActivationCommand(Guid Id) : ICommand<IResult>;

/// <summary>
/// Validator for <see cref="ToggleEmployeeActivationCommand"/>.
/// </summary>
internal sealed class ToggleEmployeeActivationCommandValidator
    : AbstractValidator<ToggleEmployeeActivationCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToggleEmployeeActivationCommandValidator"/> class.
    /// </summary>
    public ToggleEmployeeActivationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
    }
}

/// <summary>
/// Handles toggling the activation status of an employee by ID.
/// </summary>
internal sealed class ToggleEmployeeActivationCommandHandler(AppDbContext dbContext)
    : ICommandHandler<ToggleEmployeeActivationCommand, IResult>
{
    /// <summary>
    /// Handles the toggle employee activation command.
    /// </summary>
    /// <param name="request">The toggle employee activation command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the toggle operation.</returns>
    public async Task<IResult> Handle(
        ToggleEmployeeActivationCommand request,
        CancellationToken cancellationToken
    )
    {
        var employee = await dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (employee is null)
            return Results.NotFound();

        employee.ToggleActivation();
        dbContext.Employees.Update(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(employee);
    }
}
