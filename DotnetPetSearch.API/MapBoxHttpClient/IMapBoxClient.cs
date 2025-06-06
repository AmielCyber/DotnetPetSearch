using DotnetPetSearch.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetPetSearch.API.MapBoxHttpClient;

/// <summary>
/// MapBox contract to get a displayable location name and zipcode from either
/// a zipcode or coordinates.
/// </summary>
public interface IMapBoxClient
{
    public Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromZipCode(string zipcode);
    public Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromCoordinates(double longitude, double latitude);
}