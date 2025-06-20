using System.ComponentModel.DataAnnotations;

namespace DotnetPetSearch.Data.Entities;

/// <summary>
///     PetFinder Token entity to be stored and retrieve from our DB for reusability(approx: 60 min).
/// </summary>
public class PetFinderToken
{
    public int Id { get; init; } = 1;
    [Required] public required string AccessToken { get; set; }

    [Required] public required DateTime ExpiresIn { get; set; }
}