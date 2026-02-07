using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UniJG_Backend.HealthChecks
{
    /// <summary>
    /// Classe responsável por alterar o retorno dos HealthChecks
    /// pra uma estrutura mais completa.
    /// </summary>
    public class CustomHealthCheckOptions : HealthCheckOptions
    {
        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public CustomHealthCheckOptions()
        {
            ResponseWriter = WriteResponse;
        }

        private static async Task WriteResponse(HttpContext context, HealthReport report)
        {
            string result = JsonSerializer.Serialize(
                    new
                    {
                        status = report.Status,
                        dependencies = report.Entries.Select(e => new
                        {
                            key = e.Key,
                            message = e.Value.Exception?.Message,
                            status = e.Value.Status,
                            description = e.Value.Description,
                            data = e.Value.Data,
                        }),
                    }, serializerOptions);
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }
    }
}
