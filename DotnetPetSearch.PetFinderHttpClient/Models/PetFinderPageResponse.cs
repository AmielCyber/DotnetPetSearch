using System.Text.Json.Serialization;

namespace DotnetPetSearch.PetFinderHttpClient.Models;

/// <summary>
///     Pagination object from PetFinder response when fetching a list of pets.
/// </summary>
/// <param name="CountPerPage">Number of pets to have in the list.</param>
/// <param name="TotalCount">Total amount of pets from the search.</param>
/// <param name="CurrentPage">Current page number.</param>
/// <param name="TotalPages">Total number of pages.</param>
public record PetFinderPageResponse(
    [property: JsonPropertyName("count_per_page")]
    int CountPerPage,
    [property: JsonPropertyName("total_count")]
    int TotalCount,
    [property: JsonPropertyName("current_page")]
    int CurrentPage,
    [property: JsonPropertyName("total_pages")]
    int TotalPages
);