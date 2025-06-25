namespace DotnetPetSearch.API.Configurations;

public class MapBoxSettings
{
    public required string Uri { get; set; }
    public IDictionary<string, string?> Options { get; set; } = new Dictionary<string, string?>();
}