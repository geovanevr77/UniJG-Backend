using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using UniJG.Application.Abstractions.Behaviors;

namespace UniJG.Application.Abstractions
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddPipelineBehaviors(this IServiceCollection services)
        {
            return services
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddScoped(typeof(IRequestExceptionHandler<,,>), typeof(ExceptionHandlerBehavior<,,>));
        }
    }
}