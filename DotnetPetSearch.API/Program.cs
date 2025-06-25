using DotnetPetSearch.API.Configurations;
using DotnetPetSearch.API.Extensions;
using DotnetPetSearch.API.Middleware;
using DotnetPetSearch.API.Services;
using DotnetPetSearch.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(configure => { configure.ReturnHttpNotAcceptable = true; });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();
var clients = builder.Configuration.GetRequiredSection(nameof(ClientUsers)).Get<Dictionary<string, string>>()!;
builder.Services.AddCors(options =>
{
    foreach (KeyValuePair<string, string> client in clients)
        options.AddPolicy(client.Key, policy => policy.WithOrigins(client.Value)
            .WithExposedHeaders("X-Pagination")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
});
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(30);
});
builder.Services.AddDbContext<PetSearchContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                              ??  throw new InvalidOperationException(
                                  "Connection string 'Default Connection not found!");
    options.UseMySQL(connectionString);
});
builder.Services.AddHostedService<TokenRefreshService>();
// Adding Services from Extensions/MyServiceCollectionExtensions
builder.Services.AddPetFinderServicesCollection(builder.Configuration);
builder.Services.AddMapBoxServicesCollection(builder.Configuration);
builder.Services.AddSwaggerGenWithOptions(builder.Configuration);
// End of Service Registration

WebApplication app = builder.Build();

{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<PetSearchContext>();
    context.Database.EnsureCreated();
}

// Set middleware pipeline
if (app.Environment.IsProduction()) app.UseHsts();

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePages();
app.UseSwagger(c =>
{
    c.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0;
});
app.UseSwaggerUI();
foreach (string clientNamePolicy in clients.Keys) app.UseCors(clientNamePolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();