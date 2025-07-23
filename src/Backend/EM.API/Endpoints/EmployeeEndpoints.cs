using EM.Application.Features.Employee.Commands;
using EM.Application.Features.Employee.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MinimalApis.Discovery;

namespace EM.API.Endpoints;

public sealed class EmployeeEndpoints : IApi
{
    public void Register(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/employees")
            .WithTags("Employees")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetAllEmployees)
            .WithName("GetEmployees")
            .WithSummary("Retrieves a list of employees")
            .WithDescription("This endpoint returns a list of all available employees in the system.")
            .Produces<string>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetEmployeeById)
            .WithName("GetEmployeeById")
            .WithSummary("Retrieves an employee by ID")
            .WithDescription("This endpoint returns a single employee based on its ID in the system.");

        group.MapPost("/", CreateEmployee)
            .WithName("CreateEmployee")
            .WithSummary("Creates a new employee")
            .WithDescription("This endpoint allows the creation of a new employee in the system.")
            .Produces<string>(StatusCodes.Status201Created);

        group.MapPut("/", UpdateEmployee)
            .WithName("UpdateEmployee")
            .WithSummary("Updates an existing employee")
            .WithDescription("This endpoint allows updating an existing employee.")
            .Produces(StatusCodes.Status200OK);

        group.MapDelete("/{id}", DeleteEmployee)
            .WithName("DeleteEmployee")
            .WithSummary("Deletes an employee")
            .WithDescription("This endpoint allows deleting an employee.")
            .Produces(StatusCodes.Status204NoContent);
    }

    public static async Task<IResult> GetAllEmployees(IMediator mediator, CancellationToken ct)
        => await mediator.Send(new GetAllEmployeesQuery(), ct);

    public static async Task<IResult> GetEmployeeById(Guid id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new GetEmployeeByIdQuery(id), ct);

    public static async Task<IResult> CreateEmployee([FromBody] CreateEmployeeCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> UpdateEmployee([FromBody] UpdateEmployeeCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> DeleteEmployee(Guid id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new DeleteEmployeeCommand(id), ct);
}
