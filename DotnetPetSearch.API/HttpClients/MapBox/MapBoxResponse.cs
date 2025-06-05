using System.Text.Json.Serialization;

namespace DotnetPetSearch.API.HttpClients.MapBox;

public class MapBoxResponse
{
    [JsonPropertyName("features")]
    public MapBoxFeatures[] Features { get; set; } = [];
}

public class MapBoxFeatures
{
    [JsonPropertyName("properties")]
    public required Properties Properties { get; set; }
}

public class Properties
{
    [JsonPropertyName("place_formatted")]
    public required string Location { get; set; }
    
    [JsonPropertyName("name")]
    public required string ZipCode { get; set; }
}