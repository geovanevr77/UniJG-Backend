using Elastic.Apm.AspNetCore.DiagnosticListener;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using UniJG_Backend.Converters;
using UniJG_Backend.Filters;
using UniJG_Backend.HealthChecks;

namespace UniJG_Backend
{
    public static partial class Bootstrapper
    {
        private static readonly string[] databaseTags = ["database"];
        private static readonly string[] apiTags = ["api"];
        private static readonly string[] environmentVariableTags = ["environment", "variables"];

        public static IServiceCollection AddAuthenticationExtension(
            this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.BackchannelHttpHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                options.Authority = Environment.GetEnvironmentVariable("SICOF_AUTH_AUTHORITY");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Environment.GetEnvironmentVariable("SICOF_AUTH_ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("SICOF_AUTH_AUDIENCE"),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SICOF_AUTH_SECRET"))),
                };
            });

            return services;
        }

        public static IServiceCollection AddControllerAndFilters(
            this IServiceCollection services,
            Action<IMvcBuilder> action = default)
        {
            IMvcBuilder mvcBuilder = services.AddControllers(options =>
            {
                options.Filters.Add(new ResponseActionFilter());
            }).ConfigureApiBehaviorOptions(setup =>
            {
                setup.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.Handle;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterDateTime());
                options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterDateTimeNullable());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            action?.Invoke(mvcBuilder);

            return services;
        }

        public static IServiceCollection AddSwagger(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "UniJG.Api",
                        Version = "0.1",
                        Description = "Setup UniJG.Api",
                        Contact = new OpenApiContact
                        {
                            Name = "UNI",
                            Url = new Uri(Environment.GetEnvironmentVariable("UNI_URL"))
                        }
                    });

                string applicationBasePath = AppContext.BaseDirectory;
                string applicationName = AppDomain.CurrentDomain.FriendlyName;
                string xmlDocumentPath = Path.Combine(applicationBasePath, $"{applicationName}.xml");

                if (File.Exists(xmlDocumentPath))
                {
                    options.IncludeXmlComments(xmlDocumentPath);
                }

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer SEU_TOKEN",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks()
                    .AddCheck<ApiHealthCheck>("UniJG.Api", null, [apiTags[0]])
                    .AddCheck<EnvironmentVariablesCheck>("UniJG.Api Environment Variables", null, [environmentVariableTags[0], environmentVariableTags[1]])
                    .AddSqlServer(Environment.GetEnvironmentVariable("CONNECTIONSTRING_UNIJG") ?? string.Empty,
                                  name: "SQL Server",
                                  failureStatus: HealthStatus.Unhealthy,
                                  tags: [databaseTags[0]]);

            services.AddScoped<ApiHealthCheck>();

            return services;
        }

        public static IServiceCollection AddElasticApmConfiguration(
            this IServiceCollection services)
        {
            services.AddElasticApm(
                new HttpDiagnosticsSubscriber(),
                new AspNetCoreDiagnosticSubscriber(),
                new EfCoreDiagnosticsSubscriber());

            return services;
        }

        public static void UseEndpointsConfiguration(this WebApplication app)
        {
            app.MapControllers();
            app.MapGet("/", () => "Ok").WithName("Probe");
        }

        public static void UseHealthChecksConfiguration(this WebApplication app)
        {
            app.MapHealthChecks("/health", new CustomHealthCheckOptions());
        }
    }
}