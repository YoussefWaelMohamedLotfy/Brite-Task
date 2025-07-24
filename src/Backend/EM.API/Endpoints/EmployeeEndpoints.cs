using EM.Application.Features.Employee.Commands;
using EM.Application.Features.Employee.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MinimalApis.Discovery;

namespace EM.API.Endpoints;

/// <summary>
/// API endpoints for managing employees.
/// </summary>
public sealed class EmployeeEndpoints : IApi
{
    /// <summary>
    /// Registers employee endpoints to the route builder.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public void Register(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/employees")
            .WithTags("Employees")
            .WithOpenApi();

        group.MapGet("/", GetAllEmployees)
            .WithName("GetEmployees")
            .WithSummary("Retrieves a list of employees")
            .WithDescription("This endpoint returns a list of all available employees in the system.")
            .Produces<string>(StatusCodes.Status200OK)
            .RequireAuthorization("HR-Viewer-Admin-Policy");

        group.MapGet("/{id}", GetEmployeeById)
            .WithName("GetEmployeeById")
            .WithSummary("Retrieves an employee by ID")
            .WithDescription("This endpoint returns a single employee based on its ID in the system.")
            .RequireAuthorization("HR-Viewer-Admin-Policy");

        group.MapPost("/", CreateEmployee)
            .WithName("CreateEmployee")
            .WithSummary("Creates a new employee")
            .WithDescription("This endpoint allows the creation of a new employee in the system.")
            .Produces<string>(StatusCodes.Status201Created)
            .RequireAuthorization("HR-Admin-Policy");

        group.MapPut("/", UpdateEmployee)
            .WithName("UpdateEmployee")
            .WithSummary("Updates an existing employee")
            .WithDescription("This endpoint allows updating an existing employee.")
            .Produces(StatusCodes.Status200OK)
            .RequireAuthorization("HR-Admin-Policy");

        group.MapDelete("/{id}", DeleteEmployee)
            .WithName("DeleteEmployee")
            .WithSummary("Deletes an employee")
            .WithDescription("This endpoint allows deleting an employee.")
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization("HR-Admin-Policy");

        group.MapPut("/{id}/toggle-activation", ToggleEmployeeActivation)
            .WithName("ToggleEmployeeActivation")
            .WithSummary("Toggles the activation status of an employee")
            .WithDescription("This endpoint enables or disables an employee based on their current activation status.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization("HR-Admin-Policy");
    }

    /// <summary>
    /// Retrieves all employees.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A list of employees.</returns>
    public static async Task<IResult> GetAllEmployees([AsParameters] GetAllEmployeesQuery request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> GetEmployeeById(Guid id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new GetEmployeeByIdQuery(id), ct);

    public static async Task<IResult> CreateEmployee([FromBody] CreateEmployeeCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> UpdateEmployee([FromBody] UpdateEmployeeCommand request, IMediator mediator, CancellationToken ct)
        => await mediator.Send(request, ct);

    public static async Task<IResult> DeleteEmployee(Guid id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new DeleteEmployeeCommand(id), ct);

    public static async Task<IResult> ToggleEmployeeActivation(Guid id, IMediator mediator, CancellationToken ct)
        => await mediator.Send(new ToggleEmployeeActivationCommand(id), ct);
}
