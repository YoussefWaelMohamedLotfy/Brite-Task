using EM.Infrastructure.Data;
using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Department.Commands;

public sealed record CreateDepartmentCommand(string Name, string? Description) : IRequest<IResult>;

internal sealed class CreateDepartmentCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<CreateDepartmentCommand, IResult>
{
    public async Task<IResult> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
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

internal sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Department name is required.");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreatedBy is required.");
    }
}
