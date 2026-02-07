using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections;

namespace UniJG_Backend.HealthChecks
{
    internal class EnvironmentVariablesCheck : IHealthCheck
    {
        /// <summary>
        /// Array com os nomes das variáveis de ambiente que serão exibidas no HealthCheck
        /// </summary>
        private readonly Dictionary<string, bool> VariaveisParaExibir = new()
        {
            ["BUILD_NUMBER"] = false,
            ["ASPNETCORE_ENVIRONMENT"] = false,
            ["CONNECTIONSTRING_UNI"] = true,
            ["ELASTIC_APM_SERVER_URL"] = false,
            ["ELASTIC_APM_SERVICE_NAME"] = false,
            ["ELASTIC_APM_API_KEY"] = true,
            ["ELASTIC_APM_ENVIRONMENT"] = false,
            ["ELASTIC_APM_VERIFY_SERVER_CERT"] = false,
            ["ELASTIC_APM_CAPTURE_BODY"] = false,
            ["UNIJG_URL"] = false,
            ["SICOF_AUTH_AUTHORITY"] = false,
            ["SICOF_AUTH_ISSUER"] = false,
            ["SICOF_AUTH_AUDIENCE"] = false,
            ["SICOF_AUTH_SECRET"] = true,
            ["SICOF_AUTH_SECRET_BACKEND"] = true
        };

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            IDictionary<string, object> variablesDictionary = MapearVariaveisParaDictionary();

            if (variablesDictionary.Count != VariaveisParaExibir.Count)
            {
                return Task.FromResult(
                   HealthCheckResult.Unhealthy(
                       description: "Listagem das Variáveis de Ambiente",
                       data: variablesDictionary as IReadOnlyDictionary<string, object>));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy(
                    description: "Listagem das Variáveis de Ambiente",
                    data: variablesDictionary as IReadOnlyDictionary<string, object>));
        }

        /// <summary>
        /// Faz o mapeamento de todas as variáveis de ambiente que devem ser exibidas para um objeto Dictionary
        /// </summary>
        private Dictionary<string, object> MapearVariaveisParaDictionary()
        {
            Dictionary<string, object> variablesDictionary = new();
            IDictionary variables = Environment.GetEnvironmentVariables();

            foreach (DictionaryEntry entry in variables)
            {
                string key = entry.Key.ToString();
                string value = entry.Value.ToString();
                bool containsKey = VariaveisParaExibir.ContainsKey(key);

                if (containsKey)
                {
                    if (VariaveisParaExibir[key])
                    {
                        value = OcultarParteDaSecret(value);
                    }

                    variablesDictionary.Add(key, value);
                }
            }

            return variablesDictionary;
        }

        private static string OcultarParteDaSecret(string value) => $"{value[..(value.Length / 3)]}...";
    }
}