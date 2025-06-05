using DotnetPetSearch.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetPetSearch.API.MapBoxHttpClient;

public interface IMapBoxClient
{
    public Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromZipCode(string zipcode);
    public Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromCoordinates(double longitude, double latitude);
}