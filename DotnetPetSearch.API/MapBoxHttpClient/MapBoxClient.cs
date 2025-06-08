using System.Net;
using Microsoft.Extensions.Options;

namespace DotnetPetSearch.API.MapBoxHttpClient;

public class MapBoxClient : IMapBoxClient
{
    private readonly HttpClient _httpClient;
    private readonly MapBoxConfiguration _options;

    public MapBoxClient(HttpClient httpClient, IOptions<MapBoxConfiguration> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<MapBoxProperties?> GetLocationFromZipCode(string zipcode)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(_options.GetZipCodeQuery(zipcode));
        return await GetClientResponse(response);
    }

    public async Task<MapBoxProperties?> GetLocationFromCoordinates(double longitude,
        double latitude)
    {
        using HttpResponseMessage response = await _httpClient
            .GetAsync(_options.GetCoordsQuery(longitude, latitude));
        return await GetClientResponse(response);
    }

    private async Task<MapBoxProperties?> GetClientResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        MapBoxResponse mapBoxResponse = await response.Content.ReadFromJsonAsync<MapBoxResponse>() ??
                             throw new HttpRequestException(
                                 $"Unable to get map box response. {response.StatusCode}. Reason: {response.ReasonPhrase}");

        return mapBoxResponse.Features.FirstOrDefault()?.MapBoxProperties;
    }
}