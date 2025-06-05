using DotnetPetSearch.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetPetSearch.API.HttpClients.MapBox;

public interface IMapBoxClient
{
    public Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromZipCode(string zipcode);
    public Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromCoordinates(double longitude, double latitude);
}