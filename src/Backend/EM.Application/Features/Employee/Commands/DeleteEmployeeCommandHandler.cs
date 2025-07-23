using EM.Infrastructure.Data;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Employee.Commands;

public sealed record DeleteEmployeeCommand(Guid Id) : IRequest<IResult>;

internal sealed class DeleteEmployeeCommandHandler(
    AppDbContext dbContext)
    : IRequestHandler<DeleteEmployeeCommand, IResult>
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
