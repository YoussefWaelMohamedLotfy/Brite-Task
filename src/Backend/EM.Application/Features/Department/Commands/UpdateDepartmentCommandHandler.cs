using EM.Infrastructure.Data;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

public sealed record UpdateDepartmentCommand(
    int Id,
    string Name,
    string? Description)
    : IRequest<IResult>;

internal sealed class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Department ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Department name is required.");
    }
}

internal sealed class UpdateDepartmentCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<UpdateDepartmentCommand, IResult>
{
    public async Task<IResult> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
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
