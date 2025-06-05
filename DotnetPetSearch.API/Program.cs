using System.Collections.Immutable;
using DotnetPetSearch.API.Configurations;
using DotnetPetSearch.API.MapBoxHttpClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MapBoxConfiguration>(mapBoxConfig =>
{
    string accessToken = builder.Configuration.GetValue<string>("MapBoxToken")!;
    Dictionary<string, string?> options = builder.Configuration.GetRequiredSection("MapBox:Options")
        .Get<Dictionary<string, string?>>()!;
    options.Add("access_token", accessToken);
    mapBoxConfig.Options = options.ToImmutableList();
});
builder.Services.AddHttpClient<IMapBoxClient, MapBoxClient>(client =>
{
    string mapBoxUri = builder.Configuration.GetRequiredSection("MapBox:Uri").Value!;
    client.BaseAddress = new Uri(mapBoxUri);
});
var app = builder.Build();

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