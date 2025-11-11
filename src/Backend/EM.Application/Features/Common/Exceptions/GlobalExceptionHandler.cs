using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EM.Application.Features.Common.Exceptions;

/// <summary>
/// Handles global exceptions and writes problem details responses.
/// </summary>
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle an exception and write a problem details response.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the exception was handled; otherwise, false.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = exception.GetType().Name,
            Detail = exception.Message,
        };

        if (exception.InnerException is not null)
        {
            problemDetails.Extensions.TryAdd(
                "InnerExceptionType",
                exception.InnerException.GetType().Name
            );
            problemDetails.Extensions.TryAdd("InnerException", exception.InnerException);
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        return await problemDetailsService.TryWriteAsync(
            new()
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails,
            }
        );
    }
}
