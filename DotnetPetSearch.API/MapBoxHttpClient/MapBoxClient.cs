using System.Net;
using DotnetPetSearch.API.Configurations;
using DotnetPetSearch.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

    public async Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromZipCode(string zipcode)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(_options.GetZipCodeQuery(zipcode));
        return await GetClientResponse(response);
    }

    public async Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromCoordinates(double longitude,
        double latitude)
    {
        using HttpResponseMessage response = await _httpClient
            .GetAsync(_options.GetCoordsQuery(longitude, latitude));
        return await GetClientResponse(response);
    }

    private async Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetClientResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return GetProblemDetails(response.StatusCode);

        MapBoxResponse? mapBoxResponse = await response.Content.ReadFromJsonAsync<MapBoxResponse>();
        if (mapBoxResponse is null)
            return GetProblemDetails(HttpStatusCode.InternalServerError);

        MapBoxFeatures? features = mapBoxResponse.Features.FirstOrDefault();
        if (features is null)
            return GetProblemDetails(HttpStatusCode.NotFound);

        return TypedResults.Ok(new LocationDto
        {
            LocationName = features.MapBoxProperties.Location,
            Zipcode = features.MapBoxProperties.ZipCode,
        });
    }

    private ProblemHttpResult GetProblemDetails(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest =>
            TypedResults.Problem(statusCode: (int)statusCode, detail: "Invalid parameters entered."),
        HttpStatusCode.NotFound =>
            TypedResults.Problem(
                statusCode: (int)statusCode,
                detail: "Location was not found. Please enter a valid zipcode or coordinates."),
        HttpStatusCode.Unauthorized =>
            throw new Exception($"MapBox access token is invalid or has expired."),
        HttpStatusCode.Forbidden =>
            throw new Exception($"MapBox responded with forbidden status code."),
        HttpStatusCode.InternalServerError =>
            throw new Exception($"MapBox server error returned."),
        _ => throw new Exception($"Unexpected Error code: {statusCode} from MapBox API."),
    };
}