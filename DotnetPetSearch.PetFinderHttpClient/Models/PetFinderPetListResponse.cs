using System.Text.Json.Serialization;

namespace DotnetPetSearch.PetFinderHttpClient.Models;

/// <summary>
///     PetList response from the PetFinder API.
///     Does not include sensitive data such as phone numbers, emails, since we will not send sensitive data to the client.
/// </summary>
public class PetFinderPetListResponse
{
    /// <summary>The pet list.</summary>
    [JsonPropertyName("animals")]
    public required List<PetFinderPet> Animals { get; init; }

    /// <summary>The pagination metadata from the PetFinder API JSON response.</summary>
    [JsonPropertyName("pagination")]
    public required PetFinderPageResponse Page { get; init; }
}