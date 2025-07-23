using EM.Application.Features.Common.Abstractions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace EM.Application.Features.Common.Behaviours;

public sealed class ValidationPipelineBehaviour<TRequest, TResult>(IValidator<TRequest> validator)
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : ICommand<TResult>
    where TResult : IResult
{
    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        return !validationResult.IsValid
            ? (TResult)Results.ValidationProblem(
                validationResult.Errors
                    .Select(e => new KeyValuePair<string, string[]>(e.PropertyName, [e.ErrorMessage]))
            )
            : await next(cancellationToken);
    }
}
