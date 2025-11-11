using EM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

/// <summary>
/// Command to create a new department.
/// </summary>
/// <param name="Name">The name of the department.</param>
/// <param name="Description">The description of the department.</param>
public sealed record CreateDepartmentCommand(string Name, string? Description) : IRequest<IResult>;

/// <summary>
/// Handles the creation of a new department.
/// </summary>
internal sealed class CreateDepartmentCommandHandler(AppDbContext dbContext)
    : IRequestHandler<CreateDepartmentCommand, IResult>
{
    /// <summary>
    /// Handles the create department command.
    /// </summary>
    /// <param name="request">The create department command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the creation operation.</returns>
    public async Task<IResult> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken
    )
    {
        var department = new Domain.Entities.Department
        {
            Name = request.Name,
            Description = request.Description,
        };

        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Created($"/Departments/{department.ID}", department);
    }
}

/// <summary>
/// Validator for <see cref="CreateDepartmentCommand"/>.
/// </summary>
internal sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDepartmentCommandValidator"/> class.
    /// </summary>
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Department name is required.");
    }
}
