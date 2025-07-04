using System.ComponentModel.DataAnnotations;
using DotnetPetSearch.PetFinderHttpClient.Models;

namespace DotnetPetSearch.API.Models;

/// <summary>
/// The Pet Object containing the adoptable pet's attributes.
/// </summary>
public class PetDto
{
    /// <summary>
    ///     ID number
    /// </summary>
    [Required]
    public int Id { get; init; }

    /// <summary>
    ///     PetFinder URL for the user to see more information than this application such as contact details.
    /// </summary>
    [Required]
    public required string Url { get; init; }

    /// <summary>
    ///     Type of pet
    /// </summary>
    /// <example>"Cat" or "Dog"</example>
    [Required]
    public required string Type { get; init; }

    /// <summary>
    ///     Age description
    /// </summary>
    ///     <example>"Baby", "Young", "Adult", or "Senior"</example>
    [Required]
    public required string Age { get; init; }

    /// <summary>
    ///     Gender value.
    /// </summary>
    /// <example>"Male", "Female", or "Unknown"</example>
    [Required]
    public required string Gender { get; init; }

    /// <summary>
    ///     Size description
    /// </summary>
    /// <example>"small", "medium", "large", or "xlarge"</example>
    [Required]
    public required string Size { get; init; }

    /// <summary>
    ///     The name of the pet.
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     Optional pet description from the author.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     List of url photos with different photo sizes for the pet, or an empty list if none are provided.
    /// </summary>
    public required IEnumerable<PhotoSizeUrl> Photos { get; init; } = [];

    /// <summary>
    ///     A primary cropped photo with four different sizes:
    /// </summary>
    /// <example>"small", "medium", "large", or "full" </example>
    public PhotoSizeUrl? PrimaryPhotoCropped { get; init; }

    /// <summary>
    ///     The current adoption status. Most likely to be "Adoptable" when fetch from route "api/pets".
    /// </summary>
    [Required]
    public required string Status { get; init; }

    /// <summary>
    ///     Distance from the requested location. Null if requested from the route "api/pets/{petId}".
    /// </summary>
    public double? Distance { get; init; }
}