using EM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

/// <summary>
/// Command to update an existing department.
/// </summary>
/// <param name="Id">The department ID.</param>
/// <param name="Name">The new name of the department.</param>
/// <param name="Description">The new description of the department.</param>
public sealed record UpdateDepartmentCommand(int Id, string Name, string? Description)
    : IRequest<IResult>;

/// <summary>
/// Validator for <see cref="UpdateDepartmentCommand"/>.
/// </summary>
internal sealed class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDepartmentCommandValidator"/> class.
    /// </summary>
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Department ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Department name is required.");
    }
}

/// <summary>
/// Handles the update of an existing department.
/// </summary>
internal sealed class UpdateDepartmentCommandHandler(AppDbContext dbContext)
    : IRequestHandler<UpdateDepartmentCommand, IResult>
{
    /// <summary>
    /// Handles the update department command.
    /// </summary>
    /// <param name="request">The update department command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the update operation.</returns>
    public async Task<IResult> Handle(
        UpdateDepartmentCommand request,
        CancellationToken cancellationToken
    )
    {
        var department = await dbContext.Departments.FindAsync([request.Id], cancellationToken);

        if (department is null)
            return Results.NotFound();

        department.Name = request.Name;
        department.Description = request.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(department);
    }
}
