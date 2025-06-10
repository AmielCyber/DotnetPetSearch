using System.ComponentModel.DataAnnotations;

namespace DotnetPetSearch.Data.Entities;

/// <summary>
/// PetFinder Token entity to be stored and retrieve from our DB for reusability(approx: 60 min).
/// </summary>
public class PetFinderToken
{
    public int Id { get; set; }
    [Required] public required string AccessToken { get; set; }
    [Required] public DateTime ExpiresIn { get; set; } = DateTime.Now.AddMinutes(55);
}