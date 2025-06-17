using System.Collections.Immutable;
using System.Reflection;
using DotnetPetSearch.API.Configurations;
using DotnetPetSearch.Data.Configurations;
using DotnetPetSearch.Data.Services;
using DotnetPetSearch.MapBoxHttpClient;
using DotnetPetSearch.PetFinderHttpClient.Services;
using Microsoft.OpenApi.Models;

namespace DotnetPetSearch.API.Extensions;

public static class MyServicesCollectionExtensions
{

    public static IServiceCollection AddMapBoxServicesCollection(this IServiceCollection services, IConfiguration configuration)
    {
        MapBoxSettings mapBoxSettings = configuration.GetRequiredSection(nameof(MapBoxSettings)).Get<MapBoxSettings>()!;
        services.Configure<MapBoxConfiguration>(mapBoxConfig =>
        {
            var accessToken = configuration.GetValue<string>("MapBoxToken")!;
            IDictionary<string, string?> options = new Dictionary<string, string?>(mapBoxSettings.Options);
            options.Add("access_token", accessToken);
            mapBoxConfig.Options = options.ToImmutableList();
        });
        services.AddHttpClient<IMapBoxClient, MapBoxClient>(client =>
        {
            client.BaseAddress = new Uri(mapBoxSettings.Uri);
        });
        return services;
    }

    public static IServiceCollection AddPetFinderServicesCollection(this IServiceCollection services, IConfiguration configuration)
    {
        PetFinderSettings petFinderSettings = configuration.GetRequiredSection(nameof(PetFinderSettings)).Get<PetFinderSettings>()!;
        services.Configure<PetFinderCredentials>(configuration.GetRequiredSection(nameof(PetFinderCredentials)));
        
        services.AddHttpClient<ITokenService, TokenService>(client =>
        {
            client.BaseAddress = new Uri($"{petFinderSettings.BaseUri}{petFinderSettings.TokenUrl}");
        });
        services.AddHttpClient<IPetFinderClient, PetFinderClient>(client =>
        {
            client.BaseAddress = new Uri($"{petFinderSettings.BaseUri}{petFinderSettings.PetSearchUrl}");
        });
        return services;
    }

    public static IServiceCollection AddSwaggerGenWithOptions(this IServiceCollection services, IConfiguration configuration
        )
    {
        PetSearchOpenApiInfo openApiInfo = configuration.GetRequiredSection(nameof(PetSearchOpenApiInfo)).Get<PetSearchOpenApiInfo>()!;
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = openApiInfo.Title,
                Description = openApiInfo.Description,
                Version = openApiInfo.Version,
                Contact = new OpenApiContact
                {
                    Name = openApiInfo.ClientAppName,
                    Url =new Uri(openApiInfo.ClientAppLink)
                },
            });
            var file = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, file));
        });
        return services;
    }

}