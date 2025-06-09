using System.Text.Json.Serialization;

namespace DotnetPetSearch.MapBoxHttpClient;

/// <summary>
///     MapBox Response object from the API (https://docs.mapbox.com/api/search/geocoding/).
///     Only properties used in this application are shown here.
/// </summary>
/// <seealso cref="https://docs.mapbox.com/api/search/search-box/">Map Box Search Box API</seealso> 
public class MapBoxResponse
{
    [JsonPropertyName("features")] public MapBoxFeatures[] Features { get; set; } = [];
}

public class MapBoxFeatures
{
    [JsonPropertyName("properties")] public required MapBoxProperties MapBoxProperties { get; set; }
}

/// <summary>
/// The Location Information retrieved from the MapBox Search API.
/// For now, the only information retrieved will only be the Location Name and zipcode.
/// </summary>
public class MapBoxProperties
{
    /// <summary>
    /// The Location name formatted: $"{City}, {State}, {Country}".
    /// </summary>
    [JsonPropertyName("place_formatted")] public required string PlaceFormatted { get; set; }

    /// <summary>
    /// The five digit US zipcode.
    /// </summary>
    [JsonPropertyName("name")] public required string Name { get; set; }
}