using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotnetPetSearch.API.Models;

/// <summary>
/// The pagination meta-data sent to the client to paged through the list of the resource.;
/// </summary>
/// <param name="CurrentPage">Current page starting at 1.</param>
/// <param name="TotalPages">The total number of pages withing the list.</param>
/// <param name="PageSize">The page size view per list.</param>
/// <param name="TotalCount">The total number of entities in the list.</param>
public record PaginationMetaData(
    [property: JsonPropertyName("currentPage")]
    [property: Required]
    int CurrentPage,
    [property: JsonPropertyName("totalPages")]
    [property: Required]
    int TotalPages,
    [property: JsonPropertyName("pageSize")]
    [property: Required]
    int PageSize,
    [property: JsonPropertyName("totalCount")]
    [property: Required]
    int TotalCount
);