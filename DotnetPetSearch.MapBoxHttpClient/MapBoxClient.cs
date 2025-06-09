using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace DotnetPetSearch.MapBoxHttpClient;

/// <summary>
/// Calls the MapBox Search API to get a location Name and its Zipcode to be displayed in the client's UI.
/// NOTE: This API (MapBoxClient) does NOT store any location information such as zipcode or coordinates.
/// </summary>
public class MapBoxClient : IMapBoxClient
{
    private readonly HttpClient _httpClient;
    private readonly MapBoxConfiguration _options;

    public MapBoxClient(HttpClient httpClient, IOptions<MapBoxConfiguration> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <summary>
    /// Gets The MapBoxProperties response from MapBox Search Forward API.
    /// </summary>
    /// <param name="zipcode">The zipcode to get the location's information.</param>
    /// <returns>MapBoxProperty if found, else null if not found.</returns>
    public async Task<MapBoxProperties?> GetLocationFromZipCode(string zipcode)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(_options.GetZipCodeQuery(zipcode));
        return await GetClientResponse(response);
    }

    /// <summary>
    /// Get the MapBoxProperties (Location Info) from some coordinates. 
    /// </summary>
    /// <param name="longitude">The latitude value.</param>
    /// <param name="latitude">The longitude value.</param>
    /// <returns>MapBoxProperty if found, else null if not found.</returns>
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