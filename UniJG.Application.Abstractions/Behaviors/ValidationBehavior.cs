using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace UniJG.Application.Abstractions.Behaviors
{
    /// <summary>
    /// Realiza automaticamente a validação de Command e Query
    /// utilizando os Validators registrados para ele e, em caso
    /// de falha na validação, já retorna um objeto do tipo Response
    /// com o status igual a BadRequest.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

        [DebuggerStepThrough]
        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            this.validators = validators;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;

            logger.LogInformation("Validating request {requestName}", requestName);

            ValidationContext<TRequest> validationContext = new(request);

            List<ValidationFailure> validationErrors = [];
            foreach (IValidator<TRequest> validator in validators)
            {
                FluentValidation.Results.ValidationResult validationResult = validator.Validate(validationContext);
                if (!validationResult.IsValid)
                {
                    validationErrors.AddRange(validationResult.Errors);
                }
            }

            if (validationErrors.Count > 0)
            {
                TResponse response = (TResponse)Activator.CreateInstance(typeof(TResponse), validationErrors);

                logger.LogInformation("Validated request {requestName} with {count} errors", requestName, validationErrors.Count);

                return response;
            }

            logger.LogInformation("Validated request {requestName} without errors", requestName);

            return await next();
        }
    }
}