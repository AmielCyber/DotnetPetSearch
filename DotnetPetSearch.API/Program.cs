using System.Collections.Immutable;
using DotnetPetSearch.Data;
using DotnetPetSearch.MapBoxHttpClient;
using DotnetPetSearch.PetFinderHttpClient.Configurations;
using DotnetPetSearch.PetFinderHttpClient.Services;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configuration (Options Pattern)
builder.Services.Configure<PetFinderCredentials>(builder.Configuration.GetRequiredSection("PetFinderCredentials"));
builder.Services.Configure<PetFinderOptions>(builder.Configuration.GetRequiredSection("PetFinderOptions"));
builder.Services.Configure<MapBoxConfiguration>(mapBoxConfig =>
{
    var accessToken = builder.Configuration.GetValue<string>("MapBoxToken")!;
    var options = builder.Configuration.GetRequiredSection("MapBox:Options").Get<Dictionary<string, string?>>()!;
    options.Add("access_token", accessToken);
    mapBoxConfig.Options = options.ToImmutableList();
});
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<PetSearchContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Could not find connection string!");
    options.UseMySQL(connectionString);
});
builder.Services.AddHttpClient<IMapBoxClient, MapBoxClient>(client =>
{
    var mapBoxUri = builder.Configuration.GetRequiredSection("MapBox:Uri").Get<string>()!;
    client.BaseAddress = new Uri(mapBoxUri);
});

builder.Services.AddScoped<ICacheTokenService, CacheTokenService>();
builder.Services.AddHttpClient<IPetFinderClient, PetFinderClient>(client =>
{
    var petFinderUri = builder.Configuration.GetRequiredSection("PetFinderOptions:BaseUri").Get<string>()!;
    client.BaseAddress = new Uri(petFinderUri);
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();