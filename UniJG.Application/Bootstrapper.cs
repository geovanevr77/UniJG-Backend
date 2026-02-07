using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UniJG.Application.Usuarios.Data;

namespace UniJG.Application
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddApplicationValidators(
            this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection AddMapster(
            this IServiceCollection services)
        {
            TypeAdapterConfig typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
            Mapper mapperConfig = new(typeAdapterConfig);
            services.AddSingleton<IMapper>(mapperConfig);

            return services;
        }

        public static IServiceCollection AddUsuarioRequest(
            this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRequest, UsuarioRequest>();
            return services;
        }
    }
}