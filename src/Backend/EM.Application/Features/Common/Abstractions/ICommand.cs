using MediatR;

namespace EM.Application.Features.Common.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>;