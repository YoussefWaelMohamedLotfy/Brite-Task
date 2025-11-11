using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

/// <summary>
/// Command to delete an employee by ID.
/// </summary>
/// <param name="Id">The employee ID.</param>
public sealed record DeleteEmployeeCommand(Guid Id) : ICommand<IResult>;

/// <summary>
/// Handles the deletion of an employee by ID.
/// </summary>
internal sealed class DeleteEmployeeCommandHandler(AppDbContext dbContext)
    : ICommandHandler<DeleteEmployeeCommand, IResult>
{
    /// <summary>
    /// Handles the delete employee command.
    /// </summary>
    /// <param name="request">The delete employee command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the delete operation.</returns>
    public async Task<IResult> Handle(
        DeleteEmployeeCommand request,
        CancellationToken cancellationToken
    )
    {
        var employee = await dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (employee is null)
            return Results.NotFound();

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

/// <summary>
/// Validator for <see cref="DeleteEmployeeCommand"/>.
/// </summary>
internal sealed class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteEmployeeCommandValidator"/> class.
    /// </summary>
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
    }
}
