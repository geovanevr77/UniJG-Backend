using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;
using UniJG.Application.Abstractions.Helpers;
using UniJG.Application.Abstractions.Interfaces;
using UniJG.Domain.Services.Autenticacoes;
using UniJG.Domain.Services.Clients;
using UniJG.Domain.Services.Interfaces;

namespace UniJG.Domain.Services
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddDomainServices(
            this IServiceCollection services)
        {
            return services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Bootstrapper).Assembly));
        }

        public static IServiceCollection AddHelperServices(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            IEnumerable<Type> types = assembly.GetTypes()
                .Where(t => typeof(IDomainService).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (Type type in types)
            {
                services.Add(new ServiceDescriptor(type, type, lifetime));
            }

            return services;
        }

        public static IServiceCollection AddTokenDeAcesso(
            this IServiceCollection services)
        {
            services.AddScoped<ITokenDeAcesso, TokenDeAcesso>();

            return services;
        }

        public static IServiceCollection AddKeyCloakClient(
           this IServiceCollection services)
        {
            services.AddHttpClient<IKeyCloakClient, KeyCloakClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(
                    Environment.GetEnvironmentVariable("UNIJG_AUTH_AUTHORITY"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                HttpClientHandler handler = new();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    object dadosCertificado = new
                    {
                        RequestUri = message.RequestUri.ToString(),
                        EffectiveDate = cert?.GetEffectiveDateString(),
                        ExpDate = cert?.GetExpirationDateString(),
                        FriendlyName = cert?.FriendlyName,
                        IssuerName = cert?.IssuerName,
                        Issuer = cert?.Issuer,
                        Subject = cert?.Subject,
                        Errors = errors,
                        ChainStatus = chain?.ChainStatus
                    };

                    string response = JsonSerializer.Serialize(dadosCertificado);
                    ElasticLogHelper.LogJsonContent("Certificado", response);

                    return true;
                };

                return handler;
            });

            return services;
        }
    }
}