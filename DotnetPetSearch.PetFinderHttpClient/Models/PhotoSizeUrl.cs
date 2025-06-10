using System.ComponentModel.DataAnnotations;

namespace DotnetPetSearch.PetFinderHttpClient.Models;

/// <summary>
///     Contains photo URLs based on sizes(small, medium, large, and full).
/// </summary>
public class PhotoSizeUrl
{
    /// <summary>Small size url location:</summary>
    [Required]
    public required string Small { get; init; }

    /// <summary>Medium size url location.</summary>
    [Required]
    public required string Medium { get; init; }

    /// <summary>Large size url location.</summary>
    [Required]
    public required string Large { get; init; }

    /// <summary>Full size url location.</summary>
    [Required]
    public required string Full { get; init; }
}