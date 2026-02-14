using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using UniJG.Domain.Model.UnitOfWork.Interface;
using UniJG.Infrastructure.Repositories.Connections;
using UniJG.Infrastructure.Repositories.Context;

namespace UniJG.Infrastructure.Repositories
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddInfrastructureRepositories(
            this IServiceCollection services)
        {
            return services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Bootstrapper).Assembly));
        }

        public static IServiceCollection AddRepositoriesWithSqlServer(
            this IServiceCollection services)
        {
            return services.AddDbContextPool<UniDbContext>(options =>
            {
#if DEBUG
                options.LogTo(Console.Write);
                options.EnableSensitiveDataLogging();
#endif
                options
                    .UseSqlServer(
                        Environment.GetEnvironmentVariable("CONNECTIONSTRING_UNI") ?? string.Empty,
                        o =>
                        {
                            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            o.EnableRetryOnFailure(
                                maxRetryCount: 10,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd:
                                [
                                    Constants.ErroConnectionTimeoutExpired,
                                    Constants.ErroConnectionSuccessfully
                                ]
                            );
                        })
                        .LogTo(
                            filter: (eventId, level) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                            logger: (eventData) =>
                            {
                                ExecutionStrategyEventData retryEventData = eventData as ExecutionStrategyEventData;
                                IReadOnlyList<Exception> exceptions = retryEventData.ExceptionsEncountered;
                                Console.WriteLine($"Retry #{exceptions.Count} with delay {retryEventData.Delay} due to error: {exceptions[exceptions.Count - 1].Message}");
                            });
            });
        }

        public static IServiceCollection AddUnitOfWork(
            this IServiceCollection services)
        {
            return services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<UniDbContext>());
        }

        public static IServiceCollection AddDbConnectionFactory(
            this IServiceCollection services)
        {
            string? connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING_UNI");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Variável CONNECTIONSTRING_UNI não configurada.");

            return services.AddTransient<IDbConnectionFactory>(_ =>
                new DbConnectionFactory(connectionString));

        }
    }
}