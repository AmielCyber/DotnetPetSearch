using System.ComponentModel.DataAnnotations;

namespace DotnetPetSearch.API.Models;

/// <summary>
/// Contains a location Information such as the zipcode and the location's name.
/// </summary>
public class LocationDto
{
    /// <summary>
    /// The five digit US zipcode.
    /// </summary>
    /// <example>92101</example>
    [Required]
    public required string Zipcode { get; init; }

    /// <summary>
    /// The location name.
    /// </summary>
    /// <example>San Diego, CA, USA 92101</example>
    [Required]
    public required string LocationName { get; init; }
}