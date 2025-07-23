using EM.Infrastructure.Data;
using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

public sealed record DeleteDepartmentCommand(int Id) : IRequest<IResult>;

internal sealed class DeleteDepartmentCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<DeleteDepartmentCommand, IResult>
{
    public async Task<IResult> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments.FindAsync([request.Id], cancellationToken);

        if (department is null)
            return Results.NotFound();

        dbContext.Departments.Remove(department);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

internal sealed class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Department ID is required.");
    }
}
