using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using UniJG.Application.Abstractions.Data;

namespace UniJG.Application.Abstractions.Behaviors
{
    /// <summary>
    /// Classe responsável por converter Exceptions não tratadas
    /// que ocorreram na execução dos Commands e Queries para
    /// objetos Response.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal class ExceptionHandlerBehavior<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TRequest : IRequest<TResponse>
        where TException : Exception
    {
        private readonly ILogger<ExceptionHandlerBehavior<TRequest, TResponse, TException>> logger;

        [DebuggerStepThrough]
        public ExceptionHandlerBehavior(ILogger<ExceptionHandlerBehavior<TRequest, TResponse, TException>> logger)
        {
            this.logger = logger;
        }

        Task IRequestExceptionHandler<TRequest, TResponse, TException>.Handle(
            TRequest request,
            TException exception,
            RequestExceptionHandlerState<TResponse> state,
            CancellationToken cancellationToken)
        {
            if (state.Handled || !typeof(TResponse).IsAssignableFrom(typeof(Response)))
            {
                return Task.CompletedTask;
            }

            string requestName = typeof(TRequest).Name;

            if (exception is ValidationException validationException)
            {
                logger.LogError(validationException, "Validation error executing request {requestName}", requestName);

                TResponse response = (TResponse)Activator.CreateInstance(typeof(TResponse), validationException);

                state.SetHandled(response);
            } else
            {
                logger.LogError(exception, "Unexpected error executing request {requestName}", requestName);

                TResponse response = (TResponse)Activator.CreateInstance(typeof(TResponse), exception);

                state.SetHandled(response);
            }

            return Task.CompletedTask;
        }
    }
}