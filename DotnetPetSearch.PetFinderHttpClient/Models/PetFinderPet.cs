using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotnetPetSearch.PetFinderHttpClient.Models;

/// <summary>
///     Pet properties extracted from PetFinder. Only non-sensitive data is extracted.
///     Properties may be added in the future, therefore a DTO must be used to avoid
///     sending unnecessary or unwanted properties to the client.
///     <seealso cref="http://www.petfinder.com/developers/v2/docs/" />
/// </summary>
public class PetFinderPet
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
    ///     <example>"Baby", "Young", "Adult", or "Senior"</example>
    /// </summary>
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
    [JsonPropertyName("primary_photo_cropped")]
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