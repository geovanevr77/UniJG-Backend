using UniJG_Backend;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddElasticApmConfiguration();
builder.Services.AddAuthenticationExtension();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddUsuarioRequest();
builder.Services.AddControllerAndFilters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
//builder.Services.AddDomainServices();
//builder.Services.AddHelperServices();
//builder.Services.AddDbConnectionFactory();
//builder.Services.AddInfrastructureRepositories();
//builder.Services.AddRepositoriesWithSqlServer();
//builder.Services.AddUnitOfWork();
//builder.Services.AddKeyCloakClient();
//builder.Services.AddTokenDeAcesso();
//builder.Services.AddPipelineBehaviors();
//builder.Services.AddApplicationValidators();
builder.Services.AddHealthChecks(builder.Configuration);
//builder.Services.AddMapster();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpointsConfiguration();
app.UseHealthChecksConfiguration();

await app.RunAsync();