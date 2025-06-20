using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DotnetPetSearch.PetFinderHttpClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetPetSearch.API.Models;

/// <summary>
///     Pet search query parameters.
/// </summary>
public class PetSearchParameters : IPetSearchParameters
{
    /// <summary>
    ///     Search for pet type
    ///     <example>'dog' or 'cat'</example>
    /// </summary>
    [Required]
    [RegularExpression(@"(?:dog|cat)$", ErrorMessage = "Only types: 'cat' and 'dog' are supported")]
    public required string Type { get; init; }

    /// <summary>
    ///     Search pets within a zipcode. Only 5-digit zipcodes are supported.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip Code must be 5 digits.")]
    [FromQuery(Name = "Zipcode")]
    public required string Location { get; init; }

    /// <summary>
    ///     Starting page number from the list.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    [DefaultValue(1)]
    public int Page { get; init; }

    /// <summary>
    ///     Distance radius from the requested zipcode.
    /// </summary>
    [Range(0, 500, ErrorMessage = "Distance must be between 0-500")]
    [DefaultValue(25)]
    public int Distance { get; init; } = 25;

    /// <summary>
    ///     Sort pet list by distance or recent.
    ///     <example>Values(- descending): 'distance', '-distance', 'recent', or '-recent'</example>
    /// </summary>
    [RegularExpression(@"^-?(recent|distance)$",
        ErrorMessage = "Only location values: 'recent' and 'distance' with optional '-' prefix are accepted")]
    [DefaultValue("distance")]
    [FromQuery(Name = "SortBy")]
    public string Sort { get; init; } = "distance";
}