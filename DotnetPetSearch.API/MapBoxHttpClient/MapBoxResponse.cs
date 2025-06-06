using System.Text.Json.Serialization;

namespace DotnetPetSearch.API.MapBoxHttpClient;

/// <summary>
/// MapBox Response object from the API (https://docs.mapbox.com/api/search/geocoding/).
/// Only properties used in this application are shown here.
/// </summary>
public class MapBoxResponse
{
    [JsonPropertyName("features")]
    public MapBoxFeatures[] Features { get; set; } = [];
}

public class MapBoxFeatures
{
    [JsonPropertyName("properties")]
    public required MapBoxProperties MapBoxProperties { get; set; }
}

public class MapBoxProperties
{
    [JsonPropertyName("place_formatted")]
    public required string Location { get; set; }
    
    [JsonPropertyName("name")]
    public required string ZipCode { get; set; }
}