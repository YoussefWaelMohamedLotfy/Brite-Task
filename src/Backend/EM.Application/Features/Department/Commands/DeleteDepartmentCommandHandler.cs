using EM.Infrastructure.Data;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

/// <summary>
/// Command to delete a department by ID.
/// </summary>
/// <param name="Id">The department ID.</param>
public sealed record DeleteDepartmentCommand(int Id) : IRequest<IResult>;

/// <summary>
/// Handles the deletion of a department by ID.
/// </summary>
public sealed class DeleteDepartmentCommandHandler(AppDbContext dbContext)
    : IRequestHandler<DeleteDepartmentCommand, IResult>
{
    /// <summary>
    /// Handles the delete department command.
    /// </summary>
    /// <param name="request">The delete department command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the delete operation.</returns>
    public async ValueTask<IResult> Handle(
        DeleteDepartmentCommand request,
        CancellationToken cancellationToken
    )
    {
        var department = await dbContext.Departments.FindAsync([request.Id], cancellationToken);

        if (department is null)
            return Results.NotFound();

        dbContext.Departments.Remove(department);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.NoContent();
    }
}

/// <summary>
/// Validator for <see cref="DeleteDepartmentCommand"/>.
/// </summary>
public sealed class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDepartmentCommandValidator"/> class.
    /// </summary>
    public DeleteDepartmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Department ID is required.");
    }
}
