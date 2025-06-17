namespace DotnetPetSearch.API.Configurations;

/// <summary>
///     Pet Finder API resource locator declared in appsettings.
/// </summary>
public class PetFinderSettings
{
    public required string BaseUri { get; init; }
    public required string PetSearchUrl { get; init; }
    public required string TokenUrl { get; init; }
}