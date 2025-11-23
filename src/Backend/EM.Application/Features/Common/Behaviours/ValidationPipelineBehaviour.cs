using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Common.Behaviours;

/// <summary>
/// Pipeline behavior for validating requests using FluentValidation before passing to the next handler.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the response.</typeparam>
public sealed class ValidationPipelineBehaviour<TRequest, TResult>(IValidator<TRequest> validator)
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : ICommand<TResult>
    where TResult : IResult
{
    /// <summary>
    /// Handles the request by validating it and either returning validation errors or passing to the next handler.
    /// </summary>
    /// <param name="message">The request to validate.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the request or validation errors.</returns>
    public async ValueTask<TResult> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResult> next,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(message, cancellationToken);

        return !validationResult.IsValid
            ? (TResult)
                Results.ValidationProblem(
                    validationResult.Errors.Select(e => new KeyValuePair<string, string[]>(
                        e.PropertyName,
                        [e.ErrorMessage]
                    ))
                )
            : await next(message, cancellationToken);
    }
}
