using System.Text.Json.Serialization;

namespace DotnetPetSearch.PetFinderHttpClient.Models;

/// <summary>
///     Pagination object from PetFinder response when fetching a list of pets.
/// </summary>
public class PetFinderPageResponse
{
    /// <summary>Number of pets to have in the list.</summary>
    [JsonPropertyName("count_per_page")]
    public required int CountPerPage { get; init; }

    /// <summary>Total amount of pets from the search.</summary>
    [JsonPropertyName("total_count")]
    public required int TotalCount { get; init; }

    /// <summary>Current page number.</summary>
    [JsonPropertyName("current_page")]
    public required int CurrentPage { get; init; }

    /// <summary>Total number of pages.</summary>
    [JsonPropertyName("total_pages")]
    public required int TotalPages { get; init; }
}