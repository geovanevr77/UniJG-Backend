using Elastic.Apm;
using Elastic.Apm.Api;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace UniJG.Application.Abstractions.Behaviors
{
    /// <summary>
    /// Classe responsável por registrar no log quando
    /// um Command ou Query é executado, e o tempo
    /// de execução dele.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

        [DebuggerStepThrough]
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;

            ISpan span = Agent.Tracer.CurrentTransaction?.StartSpan(requestName, "app", "request");

            logger.LogInformation("Executing request {requestName}", requestName);

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                return await next();
            } catch (Exception ex)
            {
                span?.CaptureException(ex);
                throw;
            } finally
            {
                stopwatch.Stop();

                logger.LogInformation("Executed request {requestName} in {elapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);

                span?.End();
            }
        }
    }
}