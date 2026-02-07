using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UniJG_Backend.HealthChecks
{
    /// <summary>
    /// Classe responsável por verificar o status da API.
    /// </summary>
    internal class ApiHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("UniJG.Api is OK"));
        }
    }
}
