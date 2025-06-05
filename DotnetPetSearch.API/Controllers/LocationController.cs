using System.ComponentModel.DataAnnotations;
using DotnetPetSearch.API.HttpClients.MapBox;
using DotnetPetSearch.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetPetSearch.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly IMapBoxClient _mapBoxClient;

    public LocationController(IMapBoxClient mapBoxClient) => _mapBoxClient = mapBoxClient;

    [HttpGet("{zipcode}")]
    public async Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromZipCodeAsync(
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip Code must be 5 digits.")]
        string zipcode
    ) => await _mapBoxClient.GetLocationFromZipCode(zipcode);
    
    [HttpGet("coordinates")]
    public async Task<Results<Ok<LocationDto>, ProblemHttpResult>> GetLocationFromCoordinatesAsync(
        [FromQuery] double longitude,
        [FromQuery] double latitude
    ) => await _mapBoxClient.GetLocationFromCoordinates(longitude, latitude);
}