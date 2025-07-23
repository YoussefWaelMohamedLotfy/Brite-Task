using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;

using FluentValidation;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

public sealed record ToggleEmployeeActivationCommand(Guid Id) : ICommand<IResult>;

internal sealed class ToggleEmployeeActivationCommandValidator : AbstractValidator<ToggleEmployeeActivationCommand>
{
    public ToggleEmployeeActivationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
    }
}

internal sealed class ToggleEmployeeActivationCommandHandler(
    AppDbContext dbContext)
    : ICommandHandler<ToggleEmployeeActivationCommand, IResult>
{
    public async Task<IResult> Handle(ToggleEmployeeActivationCommand request, CancellationToken cancellationToken)
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
