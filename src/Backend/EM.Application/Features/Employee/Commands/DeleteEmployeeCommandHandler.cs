using EM.Application.Features.Common.Abstractions;
using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace EM.Application.Features.Employee.Commands;

public sealed record DeleteEmployeeCommand(Guid Id) : ICommand<IResult>;

internal sealed class DeleteEmployeeCommandHandler(
    AppDbContext dbContext)
    : ICommandHandler<DeleteEmployeeCommand, IResult>
{
    public async Task<IResult> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FindAsync([request.Id], cancellationToken);

        if (employee is null)
            return Results.NotFound();

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

internal sealed class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee ID is required.");
    }
}
