using System.ComponentModel.DataAnnotations;
using DotnetPetSearch.API.Extensions;
using DotnetPetSearch.API.MapBoxHttpClient;
using DotnetPetSearch.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetPetSearch.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly IMapBoxClient _mapBoxClient;

    public LocationController(IMapBoxClient mapBoxClient)
    {
        _mapBoxClient = mapBoxClient;
    }

    [HttpGet("zipcode/{zipcode}")]
    public async Task<Results<NotFound<string>, Ok<LocationDto>>> GetLocationFromZipCodeAsync(
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip Code must be 5 digits.")]
        string zipcode
    )
    {
        MapBoxProperties? response = await _mapBoxClient.GetLocationFromZipCode(zipcode);
        if (response is null)
            return TypedResults.NotFound($"Zip Code {zipcode} not found.");
        return TypedResults.Ok(response.ToLocationDto());
    }

    [HttpGet("coordinates")]
    public async Task<Results<NotFound<string>, Ok<LocationDto>>> GetLocationFromCoordinatesAsync(
        [FromQuery] double longitude,
        [FromQuery] double latitude
    )
    {
        MapBoxProperties? response = await _mapBoxClient.GetLocationFromCoordinates(longitude, latitude);
        if (response is null)
            return TypedResults.NotFound("Coordinates not found.");
        return TypedResults.Ok(response.ToLocationDto());
    }
}