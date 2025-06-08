using System.ComponentModel.DataAnnotations;

namespace DotnetPetSearch.API.Models;

public class LocationDto
{
    [Required] public required string Zipcode { get; init; }

    [Required] public required string LocationName { get; init; }
}