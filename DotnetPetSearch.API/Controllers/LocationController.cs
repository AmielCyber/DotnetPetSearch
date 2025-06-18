using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using DotnetPetSearch.API.Extensions;
using DotnetPetSearch.API.Models;
using DotnetPetSearch.MapBoxHttpClient;
using Microsoft.AspNetCore.Mvc;

namespace DotnetPetSearch.API.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status200OK)]
public class LocationController : ControllerBase
{
    private readonly IMapBoxClient _mapBoxClient;

    public LocationController(IMapBoxClient mapBoxClient)
    {
        _mapBoxClient = mapBoxClient;
    }

    /// <summary>
    /// Get a Location's Info from a zipcode
    /// </summary>
    /// <param name="zipcode">The zipcode you want to get the location's detail.</param>
    /// <returns>The Location Information of a zipcode.</returns>
    [HttpGet("zipcode/{zipcode}")]
    public async Task<ActionResult<LocationDto>> GetLocationByZipCodeAsync(
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip Code must be 5 digits.")]
        string zipcode
    )
    {
        MapBoxProperties? response = await _mapBoxClient.GetLocationFromZipCode(zipcode);
        if (response is null)
            return NotFound();
        return Ok(response.ToLocationDto());
    }

    /// <summary>
    /// Get a Location's Info from some coordinates.
    /// </summary>
    /// <param name="longitude">The longitude of the coordinates.</param>
    /// <param name="latitude">The latitude of the coordinates.</param>
    /// <returns>The Location Information of some coordinates.</returns>
    [HttpGet("coordinates")]
    public async Task<ActionResult<LocationDto>> GetLocationByCoordinatesAsync(
        [FromQuery] double longitude,
        [FromQuery] double latitude
    )
    {
        MapBoxProperties? response = await _mapBoxClient.GetLocationFromCoordinates(longitude, latitude);
        if (response is null)
            return NotFound();
        return Ok(response.ToLocationDto());
    }
}